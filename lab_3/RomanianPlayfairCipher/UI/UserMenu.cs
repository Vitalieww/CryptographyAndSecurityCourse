using RomanianPlayfairCipher.Models;
using RomanianPlayfairCipher.Services;

namespace RomanianPlayfairCipher.UI
{
    public class UserMenu
    {
        public void Display()
        {
            bool toContinue = true;
            while (toContinue)
            {
                Console.WriteLine("Romanian Playfair Cipher");
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1. Encrypt");
                Console.WriteLine("2. Decrypt");
                Console.WriteLine("3. Exit");
                string? raw = Console.ReadLine();
                if (raw == null)
                {
                    break;
                }
                string input = raw.Trim().Trim('\uFEFF');
                int.TryParse(input, out int userChoice);
                if (userChoice == 1)
                {
                    DisplayEncrypt();
                }
                else if (userChoice == 2)
                {
                    DisplayDecrypt();
                }
                else if (userChoice == 3)
                {
                    toContinue = false;
                }
                else
                {
                    Console.WriteLine("Please choose a valid option.");
                }
            }
        }
        public void DisplayEncrypt()
        {
            string message = ReadValidMessage();
            string key = ReadValidKey();
            PlayfairMatrix matrix = new(key);
            string encryptedMessage = PlayfairCipher.Encrypt(message, matrix);
            matrix.DisplayMatrix();
            Console.WriteLine($"Encrypted Message: {encryptedMessage}\n");
        }

        public void DisplayDecrypt()
        {
            string message = ReadValidCiphertext();
            string key = ReadValidKey();
            PlayfairMatrix matrix = new(key);
            string decryptedMessage = PlayfairCipher.Decrypt(message, matrix);
            matrix.DisplayMatrix();
            Console.WriteLine($"Decrypted Message: {decryptedMessage}\n");
        }
        private string ReadValidKey()
        {
            while (true)
            {
                Console.Write("Enter key (min 7 chars): ");
                string input = Console.ReadLine() ?? string.Empty;
                string normalized = AlphabetConfig.Normalize(input.Trim());

                if (AlphabetConfig.IsValidKey(normalized, out string error))
                {
                    return normalized;
                }

                Console.WriteLine(error);
            }
        }

        private string ReadValidMessage()
        {
            while (true)
            {
                Console.WriteLine("Enter the message: ");
                string input = Console.ReadLine() ?? string.Empty;
                string normalized = AlphabetConfig.Normalize(input.Trim().Replace(" ", string.Empty));

                if (AlphabetConfig.IsValidMessage(normalized, out string error))
                {
                    return normalized;
                }
                Console.WriteLine(error);
            }
        }

        private string ReadValidCiphertext()
        {
            while (true)
            {
                Console.Write("Enter ciphertext (even length): ");
                string input = Console.ReadLine() ?? string.Empty;
                string normalized = AlphabetConfig.Normalize(input.Trim().Replace(" ", string.Empty));

                if (!AlphabetConfig.IsValidMessage(normalized, out string error))
                {
                    Console.WriteLine(error);
                    continue;
                }

                if (normalized.Length % 2 != 0)
                {
                    Console.WriteLine("Ciphertext must have an even number of characters!");
                    continue; // Asks again immediately
                }

                return normalized;
            }
        }

    }
}
