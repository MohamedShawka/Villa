namespace VillaApi.Logging
{
    public class Logging: ILogging
    {
        public void LogError(string message, string Type)
        {
            if(Type== "Error")
            Console.WriteLine("Error - "+message);
            else
            {
                Console.WriteLine(message);
            }
        }
    }
}
