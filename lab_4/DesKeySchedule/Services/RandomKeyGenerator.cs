using System;
using System.Security.Cryptography;

namespace DesKeySchedule.Services
{
    public class RandomKeyGenerator
    {
        public static bool NextBit()
        {
            byte[] singleByte = new byte[1];
            RandomNumberGenerator.Fill(singleByte);
            return (singleByte[0] & 1) == 1;
        }

        public static byte[] NextBits(int totalBits)
        {
            if (totalBits <= 0) throw new ArgumentException("Bit count must be greater than zero.");
            int totalBytes = (totalBits + 7) / 8; // Ceiling division
            byte[] buffer = new byte[totalBytes];
            RandomNumberGenerator.Fill(buffer);
            // Mask off any excess bits in the final byte if totalBits isn't a multiple of 8
            int remainder = totalBits % 8;
            if (remainder != 0)
            {
                byte mask = (byte)((1 << remainder) - 1);
                buffer[buffer.Length - 1] &= mask;
            }

            return buffer;
        }
    }
}