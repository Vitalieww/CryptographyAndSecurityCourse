using System;
using System.Text;
using MonoalphabeticCryptanalysis.UI;

namespace MonoalphabeticCryptanalysis
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            UserMenu menu = new();
            menu.Display();
        }
    }
}
