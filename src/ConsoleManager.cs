namespace Vault
{
    class ConsoleManager
    {
        public static void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }

        public static string GetInput()
        {
            Console.Write("\n>>");
            string? result = Console.ReadLine();

            if (result == "")
                result = ".";

            return (result == null ? "" : result).Split(" ", StringSplitOptions.RemoveEmptyEntries)[0];
        }
    }
}
