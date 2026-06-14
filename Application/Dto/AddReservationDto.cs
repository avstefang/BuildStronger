namespace Application.Dto;

public class AddReservationDto(string email, Guid lessonId, DateTime reservationDate, Guid? equipmentSpotId = null)
{
    public string Email { get; private set; } = email;
    public Guid LessonId { get; private set; } = lessonId;
    public DateTime ReservationDate { get; private set; } = reservationDate;
    public Guid? EquipmentSpotId { get; private set; } = equipmentSpotId;
}
