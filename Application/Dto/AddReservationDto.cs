namespace Application.Dto;

public class AddReservationDto(string email, Guid lessonId, DateTime reservationDate, int? rowNumber = null, int? spotNumber = null)
{
    public string Email { get; private set; } = email;
    public Guid LessonId { get; private set; } = lessonId;
    public DateTime ReservationDate { get; private set; } = reservationDate;

    // For equipment lessons (e.g. spinning) the athlete picks a spot by its grid position.
    // Both are null for lessons whose workout needs no equipment.
    public int? RowNumber { get; private set; } = rowNumber;
    public int? SpotNumber { get; private set; } = spotNumber;
}
