using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace xor_deneme
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==XOR Şifreleme/Şifre Çözme Programı==");
                Console.WriteLine("1 - metni şifrele(string->base64");
                Console.WriteLine("2 - metni şifre çöz(base64->string)");
                Console.WriteLine("3 -  Dosyayı şifrele(plaintext->base64 Dosya)");
                Console.WriteLine("4 - Dosyanın şifresini çöz(base64 dosya->plaintext)");
                Console.WriteLine("5 - Çıkış");
                Console.Write("Seçiminiz: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        EncryptTextFlow();
                        break;
                    case "2":
                        DecryptTextFlow();
                        break;
                    case "3":
                        EncryptFileFlow();
                        break;
                    case "4":
                        DecryptFileFlow();
                        break;
                    case "5":
                        Console.WriteLine("Çıkış yapılıyor...");
                        Environment.Exit(0);
                        return;
                }
                Console.WriteLine("\nDevam etmek için bir tuşa basın...");
                Console.ReadKey();
            }
        }
        static void EncryptTextFlow()
        {
            Console.Write("\nŞifrelenecek metni girin: ");
            string plain = Console.ReadLine() ?? "";
            Console.Write("Anahtarı(Key)girin(Örnek:mysecret): ");
            string key = Console.ReadLine() ?? "";

            var cipherbase64 = EncryptToBase64(plain, key);
            Console.WriteLine("\nŞifrelenmiş (Base64): ");
            Console.WriteLine(cipherbase64);

            Console.Write("\nİstersen Bunu Dosyaya Kaydet (Y/N)? ");
            var yn = Console.ReadLine();
            if (yn?.ToLower() == "y")
            {
                Console.Write("Dosya Adı (Örnek: cipher.txt: ");
                var filename = Console.ReadLine() ?? "cipher.txt";
                File.WriteAllText(filename, cipherbase64);
                Console.WriteLine($"Şifrelenmiş metin {filename} dosyasına kaydedildi.");
            }

        }
        static void DecryptTextFlow()
        {
            Console.WriteLine("\nŞifrelenmiş Base63 metni girin veya dosya yolunu yaz(dosya: @path):");
            string input = Console.ReadLine() ?? "";

            if (input.StartsWith("@"))//dosya belirtmek için @ kullan
            {
                var path = input.Substring(1);
                if(!File.Exists(path))
                {
                    Console.WriteLine("Dosya bulunamadı.");
                    return;
                }
                input = File.ReadAllText(path);
            }
            Console.Write("Anahtarı(Key) girin: ");
            string key = Console.ReadLine() ?? "";
            try
            {
                var plain = DecryptFromBase64(input, key);
                Console.WriteLine("\nÇözülen Metin: ");
                Console.WriteLine(plain);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Çözme Hatası" + ex.Message);
            }
        }
        static void EncryptFileFlow()
        {
            Console.Write("\nŞifreli (base64) dosya yolu");
            var src = Console.ReadLine() ?? "";
            if(!File.Exists(src))
            {
                Console.WriteLine("Dosya bulunamadı.");
                return;
            }

            Console.Write("Anahtarı(Key) girin: ");
            var key = Console.ReadLine() ?? "";

            var bytes = File.ReadAllBytes(src);
            var cipherBytes = XorBytesWithKey(bytes, Encoding.UTF8.GetBytes(key));
            var base64 = Convert.ToBase64String(cipherBytes);

            Console.Write("Çıktı Dosya Adı: ");
            var outfn = Console.ReadLine() ?? "cipher.txt";
            File.WriteAllText(outfn, base64);
            Console.WriteLine($"Şifrelenmiş dosya kaydedildi: {Path.GetFullPath(outfn)}");
        }
        static void DecryptFileFlow() 
        {
            Console.Write("\nŞifreli (base64) dosya yolu: ");
            var src = Console.ReadLine() ?? "";
            if (!File.Exists(src))
            {
                Console.WriteLine("Dosya bulunamadı.");
                return;
            }
            Console.Write("Anahtarı(Key) girin: ");
            var key = Console.ReadLine() ?? "";

            try
            {
                var base64 = File.ReadAllText(src);
                var cipherBytes = Convert.FromBase64String(base64);
                var plainBytes = XorBytesWithKey(cipherBytes, Encoding.UTF8.GetBytes(key));
            }
            catch (FormatException)
            {
                Console.WriteLine("Dosya base64 formatında değil ya da bozuk");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hata: " + ex.Message);
            }
        }
        // Temel Şifreleme/Çözme Fonksiyonları
        static string EncryptToBase64(string plain, string key)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plain);
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var cipherBytes = XorBytesWithKey(plainBytes, keyBytes);
            return Convert.ToBase64String(cipherBytes);
        }
        static string DecryptFromBase64(string base64cipher, string key)
        {
            var cipherBytes = Convert.FromBase64String(base64cipher);
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var plainBytes = XorBytesWithKey(cipherBytes, keyBytes);
            return Encoding.UTF8.GetString(plainBytes);
        }
        // XOR'un uyguladığı çekirdek fonksiyon:
        static byte[] XorBytesWithKey(byte[] data, byte[] key)
        {
            if (key == null || key.Length == 0)
                throw new ArgumentException("Anahtar boş olamaz!");

            var outBytes = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                outBytes[i] = (byte)(data[i] ^ key[i % key.Length]); // anahtar tekrarlanarak kullanılır
            }
            return outBytes;
        }
    }
}
