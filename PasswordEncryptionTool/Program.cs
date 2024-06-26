using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PasswordEncryptionTool
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            const string PasswordEncryptionMenuItem = "3";
            const string PasswordDecryptionMenuItem = "2";

            Console.WriteLine(
                "Добро пожаловать в приложение для шифрования/дешифрования паролей, сохранённых в txt файле");
            Console.WriteLine($"{PasswordEncryptionMenuItem} - Зашифровать пароли");
            Console.WriteLine($"{PasswordDecryptionMenuItem} - Дешифровать пароли");

            string userInput = Console.ReadLine();

            Console.Write("Введите путь до txt файла с паролями: ");
            string filePath = Console.ReadLine();

            Console.Write(
                "Введите ваш уникальный пароль для шифрования (НИКОМУ ЕГО НЕ ПЕРЕДАВАЙТЕ И НЕ ПОКАЗЫВАЙТЕ): ");
            string password = Console.ReadLine();

            string projectDirectory = GetProjectDirectory();
            string salt;

            switch (userInput)
            {
                case PasswordEncryptionMenuItem:
                    Console.Write("Введите соль (дополнительная строка для улучшения шифрования): ");
                    salt = Console.ReadLine();
                    string encryptedFilePath = Path.Combine(projectDirectory, "encrypted_passwords.txt");
                    EncryptFile(filePath, encryptedFilePath, password, salt);
                    break;

                case PasswordDecryptionMenuItem:
                    Console.Write("Введите соль, которая была использована при шифровании: ");
                    salt = Console.ReadLine();
                    string decryptedFilePath = Path.Combine(projectDirectory, "decrypted_passwords.txt");
                    DecryptFile(filePath, decryptedFilePath, password, salt);
                    break;

                default:
                    Console.WriteLine("Такого мы не умеем...");
                    break;
            }
        }

        public static string GetProjectDirectory()
        {
            return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\"));
        }

        public static void EncryptFile(string inputFilePath, string outputFilePath, string password, string salt)
        {
            try
            {
                using (Aes aesAlgorithm = Aes.Create())
                {
                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt), 10000);
                    aesAlgorithm.Key = key.GetBytes(32);
                    aesAlgorithm.IV = key.GetBytes(16);

                    using (ICryptoTransform encryptor = aesAlgorithm.CreateEncryptor(aesAlgorithm.Key, aesAlgorithm.IV))
                    using (FileStream fileStream = new FileStream(outputFilePath, FileMode.Create))
                    using (CryptoStream cryptoStream = new CryptoStream(fileStream, encryptor, CryptoStreamMode.Write))
                    using (FileStream inputStream = new FileStream(inputFilePath, FileMode.Open))
                    {
                        inputStream.CopyTo(cryptoStream);
                        Console.WriteLine("Файл успешно зашифрован и сохранён как " + outputFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при шифровании файла: " + ex.Message);
            }
        }

        public static void DecryptFile(string inputFilePath, string outputFilePath, string password, string salt)
        {
            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt), 10000);
                    aesAlg.Key = key.GetBytes(32);
                    aesAlg.IV = key.GetBytes(16);

                    using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                    using (FileStream fileStream = new FileStream(inputFilePath, FileMode.Open))
                    using (CryptoStream cryptoStream = new CryptoStream(fileStream, decryptor, CryptoStreamMode.Read))
                    using (FileStream outputStream = new FileStream(outputFilePath, FileMode.Create))
                    {
                        cryptoStream.CopyTo(outputStream);
                        Console.WriteLine("Файл успешно расшифрован и сохранён как " + outputFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при дешифровании файла: " + ex.Message);
            }
        }
    }
}