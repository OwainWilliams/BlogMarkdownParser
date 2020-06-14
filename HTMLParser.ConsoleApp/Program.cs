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

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_configuration)
                .CreateLogger();

           

            // Get list of urls from an RSS feed.
            List<string> listOfLinks = await new RssFeedProcessor(rssFeedUrl).GetFeedLinksAsync();
            Log.Information("What are you trying to tell me? That I can dodge bullets?");
            await new HtmlProcessor(localExportPath, domain).ProcessLinks(listOfLinks);

            // TODO: Once all files are saved, git push them to a private repo. 

            Log.Information("No, Neo. I'm trying to tell you that when you're ready, you won't have to.");
            
            Console.ReadLine();
        }
    }
}