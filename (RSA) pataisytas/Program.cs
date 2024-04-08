using System;
using System.IO;
using System.Numerics;
using System.Text;

class RSAExampleWithSwitch
{
    private static readonly string EncryptedFilePath = "encrypted.txt";
    private static readonly string PublicKeyPath = "publicKey.txt";

    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("\nPasirinkimas:");
            Console.WriteLine("1. Sifruoti teksta");
            Console.WriteLine("2. Desifruoti teksta");
            Console.WriteLine("3. Iseiti");
            Console.Write("Pasirinkimas: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    EncryptText();
                    break;
                case "2":
                    DecryptTextWithPublicKey();
                    break;
                case "3":
                    Console.WriteLine("Exiting program.");
                    return;
                default:
                    Console.WriteLine("Invalid choice, please try again.");
                    break;
            }
        }
    }

    static void EncryptText()
    {
        Console.WriteLine("\nEnter prime number p:");
        BigInteger p = BigInteger.Parse(Console.ReadLine());

        Console.WriteLine("Enter prime number q:");
        BigInteger q = BigInteger.Parse(Console.ReadLine());

        BigInteger n = p * q;
        BigInteger phi = (p - 1) * (q - 1);
        BigInteger e = GenerateE(phi);

        File.WriteAllText(PublicKeyPath, $"{e},{n}");

        Console.WriteLine("Enter text to encrypt:");
        string text = Console.ReadLine();

        byte[] textBytes = Encoding.ASCII.GetBytes(text);
        StringBuilder encryptedTextBuilder = new StringBuilder();
        foreach (var byteValue in textBytes)
        {
            BigInteger encryptedByte = BigInteger.ModPow(byteValue, e, n);
            encryptedTextBuilder.Append($"{encryptedByte} ");
        }

        string encryptedText = encryptedTextBuilder.ToString().TrimEnd();
        File.WriteAllText(EncryptedFilePath, encryptedText);
        Console.WriteLine("Text encrypted and saved to file.");
        Console.WriteLine($"Encrypted text: {encryptedText}");
    }

    static void DecryptTextWithPublicKey()
    {
        if (!File.Exists(EncryptedFilePath) || !File.Exists(PublicKeyPath))
        {
            Console.WriteLine("\nEncrypted file or public key file not found.");
            return;
        }

        string encryptedText = File.ReadAllText(EncryptedFilePath);
        string[] encryptedBytes = encryptedText.Split(' ');
        StringBuilder decryptedTextBuilder = new StringBuilder();

        string publicKeyContents = File.ReadAllText(PublicKeyPath);
        string[] publicKeyParts = publicKeyContents.Split(',');
        BigInteger e = BigInteger.Parse(publicKeyParts[0]);
        BigInteger n = BigInteger.Parse(publicKeyParts[1]);

        // Pirminiu skaiciu radimas is n
        (BigInteger p, BigInteger q) = FindPandQ(n);
        if (p == 0 && q == 0)
        {
            Console.WriteLine("Failed to factorize n into prime numbers.");
            return;
        }

        BigInteger phi = (p - 1) * (q - 1);
        BigInteger d = ModInverse(e, phi);

        foreach (var encryptedByte in encryptedBytes)
        {
            BigInteger decryptedByte = BigInteger.ModPow(BigInteger.Parse(encryptedByte), d, n);
            decryptedTextBuilder.Append((char)(byte)decryptedByte);
        }

        Console.WriteLine($"\nDecrypted text: {decryptedTextBuilder.ToString()}");
    }

private static (BigInteger, BigInteger) FindPandQ(BigInteger n)
    {
        for (BigInteger i = 2; i < n; i++)
        {
            if (n % i == 0) return (i, n / i);
        }
        return (0, 0); 
    }

    private static BigInteger GenerateE(BigInteger phi)
    {
        BigInteger e = 3;
        while (GCD(e, phi) != 1)
        {
            e += 2;
        }
        return e;
    }

    private static BigInteger ModInverse(BigInteger a, BigInteger m)
    {
        BigInteger m0 = m, y = 0, x = 1;

        if (m == 1) return 0;

        while (a > 1)
        {
            BigInteger q = a / m;
            (m, a) = (a % m, m);
            (y, x) = (x - q * y, y);
        }

        return x < 0 ? x + m0 : x;
    }

    private static BigInteger GCD(BigInteger a, BigInteger b)
    {
        while (b != 0)
        {
            BigInteger temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }
}
