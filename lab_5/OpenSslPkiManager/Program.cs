using OpenSslPkiManager.UI;

namespace OpenSslPkiManager
{
    class Program
    {
        static void Main(string[] args)
        {
            var menu = new ConsoleMenu();

            if (args.Length > 0 && (args[0] == "--demo" || args[0] == "-d"))
            {
                // Trigger automated demonstration directly
                var method = typeof(ConsoleMenu).GetMethod("HandleAutomatedDemo", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                method?.Invoke(menu, null);
                return;
            }

            menu.Display();
        }
    }
}
