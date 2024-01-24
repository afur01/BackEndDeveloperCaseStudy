using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ParsingJsonFile
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            string jsonFilePath = @"D:\WEB\Rasgele Kod\ParsingJsonFile\response.json";  // jspn dosya yolu
            
            string outputFilePath = @"D:\WEB\Rasgele Kod\ParsingJsonFile\ticket.txt";  // çıktı dosya yolu

            JsonParser.ParseAndWriteToJsonFile(jsonFilePath, outputFilePath);

            Console.ReadLine();
        }

        public class JsonParser
        {
            public static void ParseAndWriteToJsonFile(string jsonFilePath, string outputFilePath)
            {
                // JSON dosyasını okur ve bir string olarak döndürüyor
                string jsonContent = File.ReadAllText(jsonFilePath);

                // JSON içeriğini parse edip JSON Array nesnesine dönüştürdük
                JArray items = JArray.Parse(jsonContent);

                // Y eksenindeki benzerlik için tolerans sınırını belirliyoruz
                int yTolerance = 10;

                // Her bir nesneyi y eksenindeki konumuna göre sıralanmış metinleri saklamak için bu koleksiyonu kullandım
                var sortedItems = new SortedList<int, List<string>>();

                //parse edilmiş JSON öğeleri üzerinde üzerinde işlem yapmak için döngüye aldık
                foreach (JObject item in items)
                {
                    // "locale": "tr" özelliğini içeren öğeleri atladık
                    if (item.ContainsKey("locale") && (string)item["locale"] == "tr")
                    {
                        continue;
                    }

                    //Her öğenin y eksenindeki konumunu y değerini alıyoruz
                    int y = (int)item["boundingPoly"]["vertices"][0]["y"];

                    // En yakın bulmak için sortedItems sözlüğündeki mevcut anahtarları kontrol ediyoruz.
                    int closestY = -1;
                    foreach (int key in sortedItems.Keys)
                    {
                        if (Math.Abs(y - key) <= yTolerance)
                        {
                            closestY = key;
                            break;
                        }
                    }

                    // Eğer yakın bir y konumu varsa mevcut satıra ekliyoruz , yoksa yeni satır oluşturuyoruz
                    if (closestY != -1)
                    {
                        sortedItems[closestY].Add((string)item["description"]);
                    }
                    else
                    {
                        sortedItems.Add(y, new List<string> { (string)item["description"] });
                    }
                }

                // Sonuçları numaralandırarak bir dosyaya yazıyoruz
                List<string> outputLines = new List<string>();
                int lineNumber = 1;
                foreach (var line in sortedItems.Values)
                {
                    outputLines.Add($"{lineNumber}: {string.Join(" ", line)}");
                    lineNumber++;
                }

                // Metin Yazma işlemi
                File.WriteAllLines(outputFilePath, outputLines);
            }
        }



    }
}
