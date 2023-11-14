using System.Security.Cryptography;
using System.Text;

namespace TaskGauge.Common
{
    public static class CryptoPassword
    {
        private const string _key = "VBU7U63L*K?HJ34V";

        public static string EncryptPassword(string password)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(_key);
            byte[] plainBytes = Encoding.UTF8.GetBytes(password);

            for (int i = 0; i < plainBytes.Length; i++)
            {
                plainBytes[i] = (byte)(plainBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return Convert.ToBase64String(plainBytes);
        }

        public static string DecryptPassword(string encryptedText)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(_key);
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                encryptedBytes[i] = (byte)(encryptedBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return Encoding.UTF8.GetString(encryptedBytes);
        }
    }




}
