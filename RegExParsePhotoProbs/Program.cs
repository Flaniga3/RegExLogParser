using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RegExParsePhotoProbs
{
    class Program
    {
        static void Main(string[] args)
        {
            Regex reg = new Regex(@"(\d\d\d\d-\d\d-\d\d \d\d:\d\d:\d\d).*((?:\d|[A-Za-z]){8}-(?:(?:\d|[A-Za-z]){4}-){3}(?:\d|[A-Za-z]){12}) - (Starting write of imageset|Recording item complete)");

            Console.WriteLine("Please provide the location of the log-files you would like to parse");
            String inputDir = Console.ReadLine();

            Console.WriteLine("Please provide the location you would like to put your parsed files");
            String outputDir = Console.ReadLine();
            Directory.CreateDirectory(outputDir);

            IEnumerable<string> files = Directory.GetFiles(inputDir);

            foreach (string file in files)
            {
                List<InfoModel> infos = new List<InfoModel>();

                using (StreamReader reader = new StreamReader($"{file}"))
                {
                    while (!reader.EndOfStream)
                    {
                        String line = reader.ReadLine();
                        if (reg.IsMatch(line))
                        {
                            Match match = reg.Match(line);
                            Guid id = new Guid(match.Groups[2].ToString());
                            InfoModel info = infos.FirstOrDefault(i => i.Id == id);
                            bool create = false;

                            if (info == null)
                            {
                                info = new InfoModel(id);
                                create = true;
                            }

                            if (match.Groups[3].ToString().StartsWith("Start"))
                                info.Start = Convert.ToDateTime(match.Groups[1].ToString());
                            else
                                info.End = Convert.ToDateTime(match.Groups[1].ToString());

                            if (create)
                                infos.Add(info);
                        }
                    }
                }

                using (StreamWriter writer = new StreamWriter($"{outputDir}\\{Guid.NewGuid()}.csv"))
                {
                    writer.WriteLine("ID, START, END, DURATION");
                    foreach (InfoModel info in infos)
                    {
                        if (info.Start != DateTime.MinValue && info.End != DateTime.MinValue)
                            writer.WriteLine($"{info.Id}, {info.Start}, {info.End}, {info.Duration}");
                    }
                }
            }

            Console.WriteLine("Finished, push any key to close.");
            Console.ReadKey();
        }
    }
}
