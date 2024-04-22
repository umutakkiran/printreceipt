using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

class Program
{
    static void Main(string[] args)
    {
        int line = 1;
        var lines = new List<dynamic>();

        try
        {
            string data = File.ReadAllText("response.json");
            var result = JsonConvert.DeserializeObject<dynamic[]>(data);
            
            Array.Reverse(result); // JSON'daki sonuçları tersine çevir
            Array.Resize(ref result, result.Length - 1); // İlk öğeyi kaldır
            Array.Reverse(result);

            var corDictionary = result
                .Select(item => new
                {
                    points = MinMaxYKoordinatiAl(item.boundingPoly.vertices),
                    description = (string)item.description
                })
                .OrderBy(item => item.points[0])
                .ToArray();

            for (int i = 0; i < corDictionary.Length; i++)
            {
                var element = corDictionary[i];
                var sentences = new List<string> { element.description };

                lines.Add(new { line = line, sentences = sentences, points = element.points });

                for (int k = i + 1; k < corDictionary.Length; k++)
                {
                    var val = corDictionary[k];

                    if (val.points[0] >= element.points[0] && val.points[0] < element.points[1])
                    {
                        i++;
                        var lineRes = lines.FirstOrDefault(el => el.line == line);
                        if (lineRes != null)
                            lineRes.sentences.Add(val.description);
                    }
                    else
                    {
                        if (k == corDictionary.Length - 1)
                        {
                            line++;
                            break;
                        }
                    }
                }
            }

            foreach (var lineItem in lines)
            {
                Console.WriteLine($"line {lineItem.line}: {string.Join(" ", lineItem.sentences)}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    static int[] MinMaxYKoordinatiAl(dynamic array)
    {
        int minY = int.MaxValue;
        int maxY = int.MinValue;
        int minX = int.MaxValue;
        int maxX = int.MinValue;

        foreach (var point in array)
        {
            minY = Math.Min(minY, (int)point.y);
            maxY = Math.Max(maxY, (int)point.y);
            minX = Math.Min(minX, (int)point.x);
            maxX = Math.Max(maxX, (int)point.x);
        }

        return new int[] { minY, maxY, minX, maxX };
    }
}