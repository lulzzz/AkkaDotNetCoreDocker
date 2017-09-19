namespace Loaner.BoudedContexts.Test
{
    public class Hello
    {
        public Hello()
        {
            Message = "Hello";
        }
        public Hello(string msg){
            Message = msg;
        }

        public string Message { get; }
    }
}