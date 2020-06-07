using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HTMLParser.Processing.Processors;

namespace HTMLParser.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Get list of urls from an RSS feed.
            List<string> listOfLinks = await new RssFeedProcessor("https://owain.codes/blog/rss/").GetFeedLinksAsync();
            Console.WriteLine("Started.....");
            await new HtmlProcessor(@"c:\temp\markdown\").ProcessLinks(listOfLinks);
           
            // TODO:
            // Once all files are saved, git push them to a private repo. 
            Console.WriteLine();
            Console.WriteLine("Finished....Press ENTER to exit.");
            Console.ReadLine();
        }
    }
}