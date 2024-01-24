using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace EncryptedRandomCode
{
    internal class Program
    {
        static void Main(string[] args)
        {

            SecureCodeGenerator secureCode = new SecureCodeGenerator();
            int numberOfcodes;

            while (true) // Sonsuz döngü 
            {
                Console.WriteLine("...");
                Console.Write("Lütfen Oluşturmak İstediğiniz Benzersiz kod adedini giriniz (0 çıkmak için) : ");
                string input = Console.ReadLine();

                if (!int.TryParse(input, out numberOfcodes)) 
                {
                    Console.WriteLine("Hatalı giriş! Lütfen bir sayı giriniz.");
                    continue; 
                }

                if (numberOfcodes == 0)
                {
                    Console.WriteLine("Program sonlandırılıyor...");
                    return; 
                }

                int counter = 0;

                while (counter < numberOfcodes)
                {
                    var code = secureCode.GenerateCode();
                    if (secureCode.CheckCode(code))
                    {
                        Console.WriteLine($"Code:{counter + 1} , {code}");
                        counter++;
                    }
                }

            }

            Console.ReadLine();
        }
    }

    public class SecureCodeGenerator
    {
        private static readonly string charSet = "ACDEFGHKLMNPRTXYZ234579";

        private static readonly Random random = new Random();

        private HashSet<string> generatedCodes = new HashSet<string>();

        // Güvenli rastgele kod üretir
        public string GenerateCode()
        {
            byte[] bytes = new byte[4]; // 4 baytlık bir rastgele sayı için  
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes); // Güvenli bir şekilde rastgele baytlar üretilmesini sağlar
            }

            var randomNumber = BitConverter.ToUInt32(bytes, 0);
            var code = new StringBuilder();

            //rastgele üretilen bir sayıyı kullanarak charSet içerisinden karakter seçmek ve bu karakterlerle bir kod oluşturmak için kullandım
            for (int i = 0; i < 8; i++)
            {
                code.Append(charSet[(int)(randomNumber % (uint)charSet.Length)]);
                randomNumber /= (uint)charSet.Length;
            }

            string finalCode = code.ToString();
            if (generatedCodes.Contains(finalCode))  
            {
                return GenerateCode();  // Bu Recursive metot ile benzersiz bir kod üretene kadar kendini tekrar edecek
            }

            generatedCodes.Add(finalCode);
            return finalCode;
        }

        // Kodun geçerliliğini kontrol eder
        public bool CheckCode(string code)
        {
            //kodun her bir karakterin ASCII değerlerinin toplamının asal sayı olup olmadığını kontrol ediyoruz
            int sum = 0;
            foreach (char c in code)
            {
                sum += c;
            }
            return IsPrime(sum);
        }

        //Verilen sayının asal olup olmadığını kontrol ediyoruz
        private bool IsPrime(int number)
        {
            if (number < 2) return false;
            for (int i = 2; i * i <= number; i++)
            {
                if (number % i == 0) return false;
            }
            return true;
        }
    }


}
