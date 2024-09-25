namespace BankOfHogwarts.Exceptions
{
    public class CantDelectException : Exception
    {
        public CantDelectException() { }

        public CantDelectException(string message)
            : base(message) { }

        public CantDelectException(string message, Exception inner)
            : base(message, inner) { }
    }
}
