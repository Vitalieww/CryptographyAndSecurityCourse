using System;
using System.Security.Cryptography;

namespace DesKeySchedule.Services
{
    public class RandomKeyGenerator
    {

        public static string RandomBitsString(int totalBits)
        {
            return RandomNumberGenerator.GetString("01", totalBits);
        }

        public static string RandomHexString(int totalHex)
        {
            string secureHexUpper = RandomNumberGenerator.GetHexString(totalHex);
            return secureHexUpper;
        }
    }
}