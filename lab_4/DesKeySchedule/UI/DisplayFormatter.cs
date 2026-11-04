using DesKeySchedule.Constants;
using ConsoleTables;
using DesKeySchedule.Services;
using System.Text;

namespace DesKeySchedule.UI
{
    public class DisplayFormatter
    {
        public static void PrintShiftSchedule()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                 TABELUL DEPLASĂRILOR CIRCULARE (SHIFT SCHEDULE)                    ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════════════╝");

            Console.Write(" Runda: ");
            for (int i = 1; i <= DesConstants.ShiftSchedule.Length; i++)
            {
                Console.Write($"{i,3} ");
            }
            Console.WriteLine();

            Console.Write(" Shift: ");
            for (int i = 0; i < DesConstants.ShiftSchedule.Length; i++)
            {
                Console.Write($"{DesConstants.ShiftSchedule[i],3} ");
            }
            Console.WriteLine("\n");
        }

        public static void PrintPc2Table()
        {
            int rows = 8;
            int cols = 6;
            Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                     TABELUL DE COMPRESIE ȘI PERMUTARE (PC-2)                       ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine("  Selectează 48 de biți din cei 56 ai blocului concatenat CiDi:\n");

            for (int r = 0; r < rows; r++)
            {
                Console.Write("   [Rand " + (r + 1) + "]  ");
                for (int c = 0; c < cols; c++)
                {
                    int index = (r * cols) + c;
                    Console.Write($"{DesConstants.PC2[index],3} ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public static void PrintRounds(List<RoundState> rounds)
        {
            if (rounds == null || rounds.Count == 0)
            {
                Console.WriteLine("Nu există runde de afișat.");
                return;
            }

            PrintShiftSchedule();
            PrintPc2Table();

            Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                        PAȘII INTERMEDIARI PÂNĂ LA RUNDA i                          ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════════════╝");

            var table = new ConsoleTable("Runda", "Shift", "Ci (28 biți)", "Di (28 biți)", "Ki (Hex)");
            foreach (RoundState round in rounds)
            {
                string hexRoundKey = BitUtils.BinaryStringToHexString(round.RoundKey);
                table.AddRow(round.RoundIndex + 1, round.BitsShift, round.LeftHalf, round.RightHalf, hexRoundKey);
            }
            table.Write();

            PrintFinalRoundSummary(rounds[^1]);
        }

        public static void PrintFinalRoundSummary(RoundState finalRound)
        {
            int roundNumber = finalRound.RoundIndex + 1;
            string keyHex = BitUtils.BinaryStringToHexString(finalRound.RoundKey);

            Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║                    REZULTAT FINAL PENTRU RUNDA i = {roundNumber,-2}                              ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine($"  • Runda solicitată (i):           {roundNumber}");
            Console.WriteLine($"  • Deplasare circulară la stânga:  {finalRound.BitsShift} bit/biți");
            Console.WriteLine($"  • Jumătatea stângă Ci (28 biți):  {finalRound.LeftHalf}");
            Console.WriteLine($"  • Jumătatea dreaptă Di (28 biți): {finalRound.RightHalf}");
            Console.WriteLine($"  • Blocul 56 biți (CiDi):          {finalRound.LeftHalf}{finalRound.RightHalf}");
            Console.WriteLine("  " + new string('-', 82));
            Console.WriteLine($"  • Cheia de rundă Ki (48 biți binar):");
            Console.WriteLine($"    {finalRound.RoundKey}");
            Console.WriteLine();
            Console.WriteLine($"  • Cheia Ki împărțită în 8 blocuri de câte 6 biți (pentru cutiile-S B1..B8):");
            
            var blocks = new StringBuilder("    ");
            for (int b = 0; b < 8; b++)
            {
                string blockBits = finalRound.RoundKey.Substring(b * 6, 6);
                blocks.Append($"B{b + 1}:[{blockBits}] ");
            }
            Console.WriteLine(blocks.ToString());
            Console.WriteLine();
            Console.WriteLine($"  • Cheia de rundă Ki în Hexazecimal (12 caractere hex):");
            Console.WriteLine($"    0x{keyHex}");
            Console.WriteLine("══════════════════════════════════════════════════════════════════════════════════════\n");
        }
    }
}
