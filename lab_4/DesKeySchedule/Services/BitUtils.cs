using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace DesKeySchedule.Services
{
    public class BitUtils
    {
        public static string HexStringToBinaryString(string hexString)
        {
            string binaryString = string.Join(string.Empty,
            hexString.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0'))
            );
            return binaryString;
        }

        public static string BinaryStringToHexString(string binaryString)
        {
            int remainder = binaryString.Length % 8;
            if (remainder != 0)
            {
                binaryString = binaryString.PadLeft(binaryString.Length + (8 - remainder), '0');
            }

            byte[] bytes = new byte[binaryString.Length / 8];
            for (int i = 0; i < bytes.Length; i++)
            {
                string byteChunk = binaryString.Substring(i * 8, 8);
                bytes[i] = Convert.ToByte(byteChunk, 2);
            }

            return Convert.ToHexString(bytes);
        }

        public static string RotateLeft(string bitString, int shifts)
        {
            char[] bitCharArray = bitString.ToCharArray();
            for (int i = 0; i < bitString.Length; i++)
            {
                bitCharArray[i] = bitString[(i + shifts) % bitString.Length];
            }
            return string.Join("", bitCharArray);
        }

        public static bool ValidateHexKey([NotNullWhen(true)] string? hexKey)
        {
            if (string.IsNullOrWhiteSpace(hexKey) || hexKey.Length != 14)
            {
                return false;
            }

            return hexKey.All(char.IsAsciiHexDigit);
        }

        public static bool ValidateBinaryKey([NotNullWhen(true)] string? binaryKey)
        {
            if (string.IsNullOrWhiteSpace(binaryKey) || binaryKey.Length != 56)
            {
                return false;
            }

            return binaryKey.All(c => c is '0' or '1');
        }
    }
}
