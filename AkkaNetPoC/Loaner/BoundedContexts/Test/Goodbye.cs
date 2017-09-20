namespace Loaner.BoundedContexts.Test
{
    public class Goodbye
    {
        public Goodbye()
        {
            Message = "Goodbye";
        }
        public Goodbye(string msg)
        {
            Message = msg;
        }

        public string Message { get; }
    }
}