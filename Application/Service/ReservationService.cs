using Application.Dto;
using Application.Interface;
using Application.Mapping;
using Application.Template;
using Domain.Entity;
using Domain.Enum;
using Domain.Exception;
using Domain.Value_object;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Service;

public class ReservationService(ILessonRepository lessonRepository, IReservationRepository reservationRepository, IAthleteRepository athleteRepository, IEquipmentSpotRepository equipmentSpotRepository, IEquipmentSpotReservationRepository equipmentSpotReservationRepository, IInstructorRepository instructorRepository, IEmailSender? emailSender = null)
{
    private readonly ILessonRepository _lessonRepository = lessonRepository;
    private readonly IReservationRepository _reservationRepository = reservationRepository;
    private readonly IAthleteRepository _athleteRepository = athleteRepository;
    private readonly IEquipmentSpotRepository _equipmentSpotRepository = equipmentSpotRepository;
    private readonly IEquipmentSpotReservationRepository _equipmentSpotReservationRepository = equipmentSpotReservationRepository;
    private readonly IInstructorRepository _instructorRepository = instructorRepository;
    private readonly IEmailSender? _emailSender = emailSender;

    // Allowances at or above this are treated as "unlimited" (the seed uses 999/9999 for those plans).
    private const int UnlimitedThreshold = 999;

    private async Task<int> CountReservationsInPeriodAsync(EmailAddress email, DateOnly start, DateOnly end)
    {
        IEnumerable<Reservation>? reservations = await _reservationRepository.GetAllReservationsByEmailAsync(email);
        return reservations?.Count(r =>
        {
            DateOnly date = DateOnly.FromDateTime(r.ReservationDate);
            return date >= start && date < end;
        }) ?? 0;
    }

    /// <summary>Credit status for the member's current period, or null when there's no active subscription.</summary>
    public async Task<GetCreditStatusDto?> GetCreditStatusAsync(string email)
    {
        EmailAddress emailAddress = new(email);
        Athlete? athlete = await _athleteRepository.GetAthleteByEmailAsync(emailAddress);
        Subscription? active = athlete?.GetActiveSubscription();
        if (active is null)
            return null;

        (DateOnly start, DateOnly end) = active.GetCreditPeriod(DateOnly.FromDateTime(DateTime.Now));
        int total = active.SubscriptionPlan.WeeklyCreditAmount;
        int used = await CountReservationsInPeriodAsync(emailAddress, start, end);

        return new GetCreditStatusDto
        {
            Total = total,
            Used = used,
            Remaining = Math.Max(0, total - used),
            Unlimited = total >= UnlimitedThreshold
        };
    }

    // EF ignores the Lesson.Instructor navigation, so populate it from each reservation's lesson
    // InstructorId before mapping — otherwise bookings show no instructor.
    private async Task PopulateInstructorsAsync(IEnumerable<Reservation> reservations)
    {
        List<Lesson> lessons = reservations
            .Select(r => r.Lesson)
            .Where(l => l is not null && l.InstructorId is not null)
            .ToList();
        if (lessons.Count == 0)
            return;

        Dictionary<Guid, Instructor> instructors = (await _instructorRepository.GetAllInstructorsAsync())
            .ToDictionary(i => i.Athlete.Id);

        foreach (Lesson lesson in lessons)
            if (lesson.InstructorId is Guid id && instructors.TryGetValue(id, out Instructor? instructor))
                lesson.AssignInstructor(instructor);
    }

    public async Task<GetReservationDto> ReserveLessonAsync(AddReservationDto dto)
    {
        EmailAddress email = new(dto.Email);
        Athlete athlete = await _athleteRepository.GetAthleteByEmailAsync(email) ??
            throw new ReservationException($"No athlete found with email {dto.Email}");
        Lesson? lesson = await _lessonRepository.GetLessonByIdAsync(dto.LessonId) ??
            throw new ReservationException($"No lesson found with id {dto.LessonId}");

        // Credit hard stop: a member needs an active subscription and an unused credit for the
        // lesson's monthly period. Unlimited plans never hit the cap. Cancelling refunds a credit
        // (cancels are hard-deleted, so they stop counting).
        Subscription activeSubscription = athlete.GetActiveSubscription() ??
            throw new ReservationException("Je hebt geen actief abonnement om mee te boeken.");

        // A cancelled subscription stays valid through its end date but no further, so a lesson
        // dated after that is out of coverage. Active subscriptions always pass this check.
        if (!activeSubscription.GrantsAccessOn(DateOnly.FromDateTime(dto.ReservationDate)))
            throw new ReservationException("Je abonnement is niet meer geldig op de datum van deze les.");

        int creditAllowance = activeSubscription.SubscriptionPlan.WeeklyCreditAmount;
        if (creditAllowance < UnlimitedThreshold)
        {
            (DateOnly periodStart, DateOnly periodEnd) = activeSubscription.GetCreditPeriod(DateOnly.FromDateTime(dto.ReservationDate));
            int creditsUsed = await CountReservationsInPeriodAsync(email, periodStart, periodEnd);
            if (creditsUsed >= creditAllowance)
                throw new ReservationException("Je hebt geen credits meer over voor deze periode.");
        }

        IEnumerable<Reservation>? reservations = await _reservationRepository.GetAllReservationsByLessonIdAsync(lesson.Id);

        Reservation reservation = new(athlete, dto.ReservationDate, lesson);
        if (reservations?.Count() < lesson.MaxCapacity)
        {
            reservation.Accept();
        }

        // A spot is only required when the lesson's room has a real spot grid (e.g. spinning bikes).
        // Equipment without assigned places (yoga mats, boxing gloves) needs no spot.
        EquipmentSpot? equipmentSpot = null;
        if (await _equipmentSpotRepository.RoomHasSpotsAsync(lesson.Room.Id))
        {
            if (dto.RowNumber is null || dto.SpotNumber is null)
                throw new ReservationException($"Lesson {lesson.Id} requires picking an equipment spot (row and spot number).");

            equipmentSpot = await _equipmentSpotRepository.GetEquipmentSpotByPositionAsync(lesson.Room.Id, dto.RowNumber.Value, dto.SpotNumber.Value) ??
                throw new ReservationException($"No equipment spot exists at row {dto.RowNumber}, spot {dto.SpotNumber} for this lesson.");

            // Reject an already-taken spot up front; the unique constraint on the join table is the final guard.
            IEnumerable<EquipmentSpotReservation>? bookedSpots = await _equipmentSpotReservationRepository.GetByLessonIdAsync(lesson.Id);
            if (bookedSpots?.Any(b => b.EquipmentSpot.Id == equipmentSpot.Id) == true)
                throw new ReservationException($"The spot at row {dto.RowNumber}, spot {dto.SpotNumber} is already booked for this lesson.");
        }

        await _reservationRepository.AddReservationAsync(reservation);

        if (equipmentSpot is not null)
        {
            EquipmentSpotReservation spotReservation = new(reservation.Id, lesson.Id, athlete.Id, equipmentSpot);
            await _equipmentSpotReservationRepository.AddEquipmentSpotReservationAsync(spotReservation);
        }

        return reservation.ToDto();
    }

    /// <summary>
    /// The confirmed (accepted) participants of a lesson — username and profile photo only —
    /// so members can see who else is attending. Waitlisted reservations are excluded.
    /// </summary>
    public async Task<IEnumerable<GetLessonParticipantDto>> GetLessonParticipantsAsync(Guid lessonId, string? excludeRequesterEmail = null)
    {
        IEnumerable<Reservation>? reservations = await _reservationRepository.GetAllReservationsByLessonIdAsync(lessonId);
        if (reservations is null)
            return [];

        // The Athlete navigation is ignored on the reservation context, so look each one up by
        // its persisted AthleteId (same pattern as PopulateInstructorsAsync uses for instructors).
        List<GetLessonParticipantDto> participants = [];
        foreach (Reservation reservation in reservations.Where(r => r.Status == ReservationStatus.Accepted))
        {
            Athlete athlete = await _athleteRepository.GetAthleteById(reservation.AthleteId);
            if (athlete.EmailAddress.ToString() != excludeRequesterEmail)
            {
                participants.Add(new GetLessonParticipantDto
                {
                    Username = athlete.Username,
                    PhotoFile = athlete.PhotoPath?.Path
                });
            }
        }

        return participants;
    }

    /// <summary>The taken spot positions for a lesson, so the app can show them as booked.</summary>
    public async Task<IEnumerable<GetBookedSpotDto>> GetBookedSpotsAsync(Guid lessonId)
    {
        IEnumerable<EquipmentSpotReservation>? bookedSpots = await _equipmentSpotReservationRepository.GetByLessonIdAsync(lessonId);
        return bookedSpots?.Select(b => new GetBookedSpotDto
        {
            RowNumber = b.EquipmentSpot.EquipmentPosition.RowNumber,
            SpotNumber = b.EquipmentSpot.EquipmentPosition.SpotNumber
        }) ?? [];
    }

    // How far before/after the lesson start an automatic check-in is accepted.
    private const int CheckInWindowMinutes = 15;

    /// <summary>
    /// Marks the member present for their reservation. Used by the app's automatic (geofence) check-in:
    /// only the owner may check in, only an accepted reservation, and only within a short window around
    /// the lesson start. Already-checked-in is a no-op so repeated polls don't error.
    /// </summary>
    public async Task CheckInAsync(Guid reservationId, string email)
    {
        Reservation reservation = await _reservationRepository.GetReservationByIdAsync(reservationId) ??
            throw new ReservationException($"No reservation found with id {reservationId}");

        Athlete athlete = await _athleteRepository.GetAthleteByEmailAsync(new EmailAddress(email)) ??
            throw new ReservationException($"No athlete found with email {email}");

        if (reservation.AthleteId != athlete.Id)
            throw new ReservationException("You can only check in to your own reservation.");

        if (reservation.Status == ReservationStatus.CheckedIn)
            return; // already present (e.g. a later poll) — nothing to do

        DateTime start = reservation.ReservationDate.Date.Add(reservation.Lesson.Schedule.StartTime.ToTimeSpan());
        DateTime now = DateTime.Now;
        if (now < start.AddMinutes(-CheckInWindowMinutes) || now > start.AddMinutes(CheckInWindowMinutes))
            throw new ReservationException($"Check-in is only possible within {CheckInWindowMinutes} minutes of the lesson start.");

        reservation.CheckIn();
        await _reservationRepository.UpdateReservationAsync(reservation);
    }

    public async Task CancelReservationAsync(Guid reservationId)
    {
        // Capture the lesson before deleting, so we can promote its waitlist afterwards.
        Reservation? reservation = await _reservationRepository.GetReservationByIdAsync(reservationId);
        Guid? lessonId = reservation?.Lesson.Id;

        // Free the equipment spot (if any) so it becomes bookable again. No-op for non-equipment lessons.
        await _equipmentSpotReservationRepository.DeleteByReservationIdAsync(reservationId);

        // Hard delete the reservation so the spot/capacity frees up and the member can book the lesson again.
        await _reservationRepository.DeleteReservationAsync(reservationId);

        // A spot just freed up — promote the next member(s) off the waitlist and notify them by email.
        if (lessonId is Guid id)
            await PromoteWaitlistForLessonAsync(id);
    }

    // Fills any free spots in a lesson from its waitlist (oldest first) and emails each promoted member.
    private async Task PromoteWaitlistForLessonAsync(Guid lessonId)
    {
        int spotsLeft = await _reservationRepository.GetReservationSpotsLeftAsync(lessonId);
        if (spotsLeft <= 0)
            return;

        IEnumerable<Reservation>? waitlisted = await _reservationRepository.GetAllWaitlistedReservationsByLessonIdAsync(lessonId);
        if (waitlisted is null)
            return;

        foreach (Reservation reservation in waitlisted.Take(spotsLeft))
        {
            reservation.Accept();
            await _reservationRepository.UpdateReservationAsync(reservation);
            await NotifyWaitlistAcceptedAsync(reservation);
        }
    }

    // Best-effort: a failed notification must never block the promotion itself.
    private async Task NotifyWaitlistAcceptedAsync(Reservation reservation)
    {
        if (_emailSender is null)
            return;

        try
        {
            Athlete athlete = await _athleteRepository.GetAthleteById(reservation.AthleteId);
            Lesson lesson = reservation.Lesson;
            DateTime start = reservation.ReservationDate.Date.Add(lesson.Schedule.StartTime.ToTimeSpan());

            EmailContentDto content = EmailTemplate.WaitlistAcceptedEmail(athlete.FullName, lesson.Workout.Name, start, lesson.Room.Name);
            await _emailSender.SendEmailAsync(athlete.FullName, athlete.EmailAddress, content.Subject, content.Body);
        }
        catch { /* swallow: notification is non-critical */ }
    }

    public async Task<IEnumerable<GetReservationDto>?> GetLessonReservationsAsync(Guid lessonId)
    {
        IEnumerable<Reservation>? reservations = await _reservationRepository.GetAllReservationsByLessonIdAsync(lessonId);
        if (reservations is null)
            return null;

        await PopulateInstructorsAsync(reservations);
        return reservations.Select(reservation => reservation.ToDto());
    }

    public async Task<IEnumerable<GetReservationDto>?> GetAthleteReservationsAsync(string email)
    {
        EmailAddress emailAddress = new(email);
        IEnumerable<Reservation>? reservations = await _reservationRepository.GetAllReservationsByEmailAsync(emailAddress);
        if (reservations is null)
            return null;

        await PopulateInstructorsAsync(reservations);
        return reservations.Select(reservation => reservation.ToDto());
    }

    // Backstop for the event-driven promotion on cancel: sweeps every lesson and fills free spots
    // from the waitlist (e.g. after a capacity change). Runs from the scheduled job.
    public async Task AutoAcceptWaitlistReservationsAsync()
    {
        IEnumerable<Lesson>? lessons = await _lessonRepository.GetAllLessonsAsync();
        foreach (Lesson lesson in lessons!)
            await PromoteWaitlistForLessonAsync(lesson.Id);
    }
}