using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HTMLParser.Processing.Processors;
using Microsoft.Extensions.Configuration;

namespace HTMLParser.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {

            IConfiguration _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var rssFeedUrl = _configuration.GetValue<string>("RssFeedUrl");
            var localExportPath = _configuration.GetValue<string>("LocalExportPath");
            var domain = _configuration.GetValue<string>("RootDomain");


            // Get list of urls from an RSS feed.
            List<string> listOfLinks = await new RssFeedProcessor(rssFeedUrl).GetFeedLinksAsync();
            Console.WriteLine("Started.....");
            await new HtmlProcessor(localExportPath).ProcessLinks(listOfLinks);

            // TODO: Once all files are saved, git push them to a private repo. 

            Console.WriteLine();
            Console.WriteLine("Finished....Press ENTER to exit.");
            Console.ReadLine();
        }
    }
}