using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        string filePath = "passwords.txt";
        string encryptedFilePath = "encrypted_passwords.txt";
        string decryptedFilePath = "decrypted_passwords.txt";
        string password = "YourStrongPassword123!"; // Пароль для шифрования и дешифрования

        EncryptFile(filePath, encryptedFilePath, password);
        DecryptFile(encryptedFilePath, decryptedFilePath, password);
    }

    static void EncryptFile(string inputFilePath, string outputFilePath, string password)
    {
        using (Aes aesAlg = Aes.Create())
        {
            var key = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes("Salt1234"), 10000);
            aesAlg.Key = key.GetBytes(32);
            aesAlg.IV = key.GetBytes(16);

            using (var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
            using (var fileStream = new FileStream(outputFilePath, FileMode.Create))
            using (var cryptoStream = new CryptoStream(fileStream, encryptor, CryptoStreamMode.Write))
            using (var inputStream = new FileStream(inputFilePath, FileMode.Open))
            {
                inputStream.CopyTo(cryptoStream);
            }
        }
    }

    static void DecryptFile(string inputFilePath, string outputFilePath, string password)
    {
        using (Aes aesAlg = Aes.Create())
        {
            var key = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes("Salt1234"), 10000);
            aesAlg.Key = key.GetBytes(32);
            aesAlg.IV = key.GetBytes(16);

            using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
            using (var fileStream = new FileStream(inputFilePath, FileMode.Open))
            using (var cryptoStream = new CryptoStream(fileStream, decryptor, CryptoStreamMode.Read))
            using (var outputStream = new FileStream(outputFilePath, FileMode.Create))
            {
                cryptoStream.CopyTo(outputStream);
            }
        }
    }
}