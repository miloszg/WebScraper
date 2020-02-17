using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebScraper.Builders;
using WebScraper.Data;
using WebScraper.Workers;

namespace WebScraper
{
    class Program
    {
        private const string Method = "search";

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Enter which city you would like to scrape info from:");
                var cityName = Console.ReadLine();

                Console.WriteLine("Enter which the CraigsList category:");
                var categoryName = Console.ReadLine();

                using (WebClient client = new WebClient())
                {
                    string content = client.DownloadString($"https://{cityName.Replace(" ", string.Empty)}.craigslist.org/{Method}/{categoryName}");

                    ScrapeCriteria scrapeCriteria = new ScrapeCriteriaBuilder()
                        .WithData(content)
                        .WithRegex(@"<a href=\""(.*?)\"" data-id=\""(.*?)\"" class=\""result-title hdrlnk\"">(.*?)</a>")
                        .WithRegexOption(RegexOptions.ExplicitCapture)
                        .WithParts(new ScrapeCriteriaPartBuilder()
                            .WithRegex(@">(.*?)</a>")
                            .WithRegexOption(RegexOptions.Singleline)
                            .Build())
                        .WithParts(new ScrapeCriteriaPartBuilder()
                            .WithRegex(@"href=\""(.*?)\""")
                            .WithRegexOption(RegexOptions.Singleline)
                            .Build())
                        .Build();

                    Scraper scraper = new Scraper();
                    var scraperdElements = scraper.Scrape(scrapeCriteria);

                    if (scraperdElements.Any())
                    {
                        foreach (var scrapedElement in scraperdElements)
                        {
                            Console.WriteLine(scrapedElement);
                        }
                    }
                    else
                    {
                        Console.WriteLine("there were no matches for scraping");
                    }
                    
                }
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            } 
        }
    }
}
