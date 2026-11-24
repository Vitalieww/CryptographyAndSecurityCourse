using DesKeySchedule.Services;

namespace DesKeySchedule.UI
{
    public class ConsoleMenu
    {
        public static void Display()
        {
            bool toContinue = true;
            while (toContinue)
            {
                Console.WriteLine("\n========================================");
                Console.WriteLine("  Welcome to step 2.3 of DES Algorithm");
                Console.WriteLine("========================================");
                Console.WriteLine("1. Enter the key in bits.");
                Console.WriteLine("2. Enter the key in hex.");
                Console.WriteLine("3. Get a random key generated.");
                Console.WriteLine("4. Exit.");
                Console.Write("Choose an option: ");

                string? choice = Console.ReadLine()?.Trim();
                if (!int.TryParse(choice, out int intChoice))
                {
                    Console.WriteLine("Invalid input. Please enter a valid number (1-4).");
                    continue;
                }

                switch (intChoice)
                {
                    case 1:
                        {
                            string bitKey = ReadValidBinaryKey();
                            int roundsAmount = AskForRounds();
                            List<RoundState> rounds = KeyScheduleService.Pc2Rounds(bitKey, roundsAmount);
                            DisplayFormatter.PrintRounds(rounds);
                            break;
                        }
                    case 2:
                        {
                            string hexKey = ReadValidHexKey();
                            int roundsAmount = AskForRounds();
                            List<RoundState> rounds = KeyScheduleService.Pc2Rounds(BitUtils.HexStringToBinaryString(hexKey), roundsAmount);
                            DisplayFormatter.PrintRounds(rounds);
                            break;
                        }
                    case 3:
                        {
                            string randomKey = RandomKeyGenerator.RandomBitsString(56);
                            Console.WriteLine($"\nThe random key generated: {randomKey}");
                            int roundsAmount = AskForRounds();
                            List<RoundState> rounds = KeyScheduleService.Pc2Rounds(randomKey, roundsAmount);
                            DisplayFormatter.PrintRounds(rounds);
                            break;
                        }
                    case 4:
                        {
                            toContinue = false;
                            Console.WriteLine("Exiting program. Goodbye!");
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("Invalid option. Please choose between 1 and 4.");
                            break;
                        }
                }
            }
        }

        private static string ReadValidBinaryKey()
        {
            while (true)
            {
                Console.Write("Write the key in bits (56 bits): ");
                string? bitKey = Console.ReadLine()?.Trim();
                if (BitUtils.ValidateBinaryKey(bitKey))
                {
                    return bitKey;
                }

                Console.WriteLine("Invalid key! The binary key must contain exactly 56 bits ('0' or '1'). Please try again.\n");
            }
        }

        private static string ReadValidHexKey()
        {
            while (true)
            {
                Console.Write("Write the key in hex (14 hex): ");
                string? hexKey = Console.ReadLine()?.Trim();
                if (BitUtils.ValidateHexKey(hexKey))
                {
                    return hexKey;
                }

                Console.WriteLine("Invalid key! The hex key must contain exactly 14 hexadecimal characters (0-9, A-F). Please try again.\n");
            }
        }

        private static int AskForRounds()
        {
            while (true)
            {
                Console.Write("Enter the amount of rounds (between 1 and 16): ");
                string? raw = Console.ReadLine()?.Trim();
                if (int.TryParse(raw, out int intAmount) && intAmount >= 1 && intAmount <= 16)
                {
                    return intAmount;
                }

                Console.WriteLine("Invalid option! Please enter a valid whole number between 1 and 16.\n");
            }
        }
    }
}
