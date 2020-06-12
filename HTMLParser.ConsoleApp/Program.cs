using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HTMLParser.Processing.Processors;
using Microsoft.Extensions.Configuration;
using Serilog;

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

            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_configuration)
                .CreateLogger();

           

            // Get list of urls from an RSS feed.
            List<string> listOfLinks = await new RssFeedProcessor(rssFeedUrl).GetFeedLinksAsync();
            logger.Information("Hold on to your hats, we're going in!");
            await new HtmlProcessor(localExportPath, domain).ProcessLinks(listOfLinks);

            // TODO: Once all files are saved, git push them to a private repo. 

            logger.Information("That's all folks!");
            
            Console.ReadLine();
        }
    }
}