using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Linq;
using SelectPdf;
using System.IO;
using System.Data;

namespace BingoKaartGenerator
{
    public class BingoKaartGenerator
    {
        private Random r;
        private string templateHtml;
        private DataRowCollection participants;
        private HtmlToPdf converter;

        public BingoKaartGenerator()
        {
            r = new Random();

            converter = new HtmlToPdf();
            converter.Options.PdfPageSize = PdfPageSize.A4;
            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            converter.Options.WebPageWidth = 0;

            templateHtml = File.ReadAllText("Template\\bingo-template.html");
            readExcelSheet();
        }

        public void GenerateAll()
        {
            for (int i = 0; i < participants.Count; i++)
            {
                var person = participants[i].ItemArray[0].ToString();
                var amount = int.Parse(participants[i].ItemArray[1].ToString());

                if (amount > 1)
                {
                    for (int j = 1; j <= amount; j++)
                        generateSinglePdf(person + $"-{j}");
                }
                else
                    generateSinglePdf(person);
            }
        }

        private void generateSinglePdf(string person)
        {
            string html = templateHtml;

            List<int> firstCard = generateBingoCardNumbers();
            List<int> secondCard = generateBingoCardNumbers();
            List<int> thirdCard = generateBingoCardNumbers();
            List<int> fourthCard = generateBingoCardNumbers();

            List<int> allNumbers = firstCard.Concat(secondCard).Concat(thirdCard).Concat(fourthCard).ToList();

            html = html.Replace("*NAAM*", person);
            
            for (int i = 1; i <= 100; i++)
            {
                int j = i - 1;
                html = html.Replace($"*{i}*", allNumbers[j].ToString());
            }

            PdfDocument doc = converter.ConvertHtmlString(html);

            doc.Save($"PDFs\\{person}.pdf");
            doc.Close();

            generateTextFile(allNumbers, person);

            Console.WriteLine($"Pdf voor {person} gereed");
        }

        public void readExcelSheet()
        {
            using (var stream = File.Open("Excel\\test-excel.xlsx", FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var ds = reader.AsDataSet();
                    participants = ds.Tables[0].Rows;
                }
            }
        }

        private List<int> generateBingoCardNumbers()
        {
            List<int> chosen = new List<int>();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    int pickedNumber;
                    switch (i)
                    {
                        case 0:
                            pickedNumber = r.Next(1, 15);
                            while (chosen.Contains(pickedNumber))
                                pickedNumber = r.Next(1, 15);
                            chosen.Add(pickedNumber);
                            break;
                        case 1:
                            pickedNumber = r.Next(16, 30);
                            while (chosen.Contains(pickedNumber))
                                pickedNumber = r.Next(16, 30);
                            chosen.Add(pickedNumber);
                            break;
                        case 2:
                            pickedNumber = r.Next(31, 45);
                            while (chosen.Contains(pickedNumber))
                                pickedNumber = r.Next(31, 45);
                            chosen.Add(pickedNumber);
                            break;
                        case 3:
                            pickedNumber = r.Next(46, 60);
                            while (chosen.Contains(pickedNumber))
                                pickedNumber = r.Next(46, 60);
                            chosen.Add(pickedNumber);
                            break;
                        case 4:
                            pickedNumber = r.Next(61, 75);
                            while (chosen.Contains(pickedNumber))
                                pickedNumber = r.Next(61, 75);
                            chosen.Add(pickedNumber);
                            break;
                    }
                }
            }
            return chosen;
        }

        private void generateTextFile(List<int> numbers, string person)
        {
            using (StreamWriter sw = new StreamWriter($"Texts\\{person}.txt"))
            {
                for (int i = 0; i < numbers.Count; i++)
                    sw.WriteLine($"{numbers[i]}");
            }
        }
    }
}
