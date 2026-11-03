using System;
using DesKeySchedule.Services;
namespace DesKeySchedule
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (RoundState round in KeyScheduleService.Pc2Rounds("11110000110011001010101011110101010101100110011110001111", 16))
            {
                Console.WriteLine("Round: " + round.RoundIndex);
                Console.WriteLine(round.LeftHalf);
                Console.WriteLine(round.RightHalf);
            }
        }
    }
}
