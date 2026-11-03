using System;
using System.Collections.Generic;
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

        private static string ShiftKeyPlus(string keyPlus, int roundIndex)
        {
            string leftPart = SeparateKeyPlus(keyPlus).leftPart;
            string rightPart = SeparateKeyPlus(keyPlus).rightPart;
            int shiftAmount = DesConstants.ShiftSchedule[roundIndex];
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

        public static RoundState Pc2(string keyPlus, int roundIndex)
        {
            int shiftAmount = DesConstants.ShiftSchedule[roundIndex];
            string keyPlusShifted = keyPlus;
            for (int i = 0; i < roundIndex + 1; i++)
            {
                keyPlusShifted = ShiftKeyPlus(keyPlusShifted, DesConstants.ShiftSchedule[i]);
            }
            string leftPart = SeparateKeyPlus(keyPlusShifted).leftPart;
            string rightPart = SeparateKeyPlus(keyPlusShifted).rightPart;
            string roundKey = PermutePc2(keyPlusShifted);
            RoundState round = new(roundIndex, shiftAmount, leftPart, rightPart, roundKey);
            return round;
        }

        public static List<RoundState> Pc2Rounds(string keyPlus, int totalRounds)
        {
            List<RoundState> rounds = [];
            for (int i = 0; i < totalRounds; i++)
            {
                RoundState round = Pc2(keyPlus, i);
                rounds.Add(round);
            }
            return rounds;
        }
    }
}