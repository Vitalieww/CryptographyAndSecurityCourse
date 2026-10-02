
namespace lab_1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            string romanianAlphabet = "AĂÂBCDEFGHIÎJKLMNOPQRSȘTȚUVWXYZ";
            // string englishAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            UserMenu menu = new(romanianAlphabet);
            menu.Display();

        }
    }
}