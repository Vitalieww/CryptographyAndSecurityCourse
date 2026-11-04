using DesKeySchedule.Constants;
using System.Text;

namespace DesKeySchedule.Services
{
    public class KeyScheduleService()
    {
        private static (string leftPart, string rightPart) SeparateKeyPlus(string keyPlus)
        {
            return (keyPlus[..28], keyPlus[28..]);
        }

        private static string ShiftKeyPlus(string keyPlus, int shiftAmount)
        {
            string leftPart = SeparateKeyPlus(keyPlus).leftPart;
            string rightPart = SeparateKeyPlus(keyPlus).rightPart;
            string leftPartShifted = BitUtils.RotateLeft(leftPart, shiftAmount);
            string rightPartShifted = BitUtils.RotateLeft(rightPart, shiftAmount);
            return string.Concat(leftPartShifted, rightPartShifted);
        }

        private static string PermutePc2(string leftAndRightShifted)
        {
            var pc2PermutedKey = new StringBuilder(string.Empty);
            for (int i = 0; i < DesConstants.PC2.Length; i++)
            {
                pc2PermutedKey.Append(leftAndRightShifted[DesConstants.PC2[i] - 1]);
            }
            return pc2PermutedKey.ToString();
        }

        public static RoundState ProcessRound(string keyPlus, int roundIndex)
        {
            int shiftAmount = DesConstants.ShiftSchedule[roundIndex];
            string keyPlusShifted = ShiftKeyPlus(keyPlus, shiftAmount);
            string leftPart = SeparateKeyPlus(keyPlusShifted).leftPart;
            string rightPart = SeparateKeyPlus(keyPlusShifted).rightPart;
            string roundKey = PermutePc2(keyPlusShifted);
            RoundState round = new(roundIndex, shiftAmount, leftPart, rightPart, roundKey);
            return round;
        }

        public static List<RoundState> Pc2Rounds(string keyPlus, int totalRounds)
        {
            List<RoundState> rounds = [];
            RoundState round;
            for (int i = 0; i < totalRounds; i++)
            {
                if (rounds.Count == 0)
                {
                    round = ProcessRound(keyPlus, i);
                }
                else
                {
                    round = ProcessRound(string.Concat(rounds[^1].LeftHalf, rounds[^1].RightHalf), i);
                }
                rounds.Add(round);
            }
            return rounds;
        }
    }
}