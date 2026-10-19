using RomanianPlayfairCipher.UI;

namespace RomanianPlayfairCipher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            UserMenu menu = new();

            menu.Display();
        }
    }
}
