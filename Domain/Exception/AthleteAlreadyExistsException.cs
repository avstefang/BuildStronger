namespace Domain.Repository
{
    public class AthleteAlreadyExistsException : System.Exception
    {
        public AthleteAlreadyExistsException()
        {
        }

        public AthleteAlreadyExistsException(string? message) : base(message)
        {
        }

        public AthleteAlreadyExistsException(string? message, System.Exception? innerException) : base(message, innerException)
        {
        }
    }
}