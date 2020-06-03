using CodeHollow.FeedReader;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace HTMLParser
{
    class Program
    {
        async static Task Main(string[] args)
        {
                 
            // Get list of urls from an RSS feed.
            List<string> listOfLinks =  ReadRssFeedAsync();
           
            HttpClient client = new HttpClient();
            HtmlDocument pageDocument = new HtmlDocument();
           
            // Configure how to handle the HTML and how to parse it.
            var config = new ReverseMarkdown.Config
            {
                UnknownTags = ReverseMarkdown.Config.UnknownTagsOption.Bypass, // Include the unknown tag completely in the result (default as well)
                GithubFlavored = true, // generate GitHub flavoured markdown, supported for BR, PRE and table tags
                RemoveComments = true, // will ignore all comments
                SmartHrefHandling = true // remove markdown output for links where appropriate
            };

            // Used to make the markdown file have a unique name.
            int id = 0;
            Console.WriteLine("Started.....");
            foreach (var link in listOfLinks)
            {
                id++;
                Console.Write("\r Importing {0} of {1} - ", id, listOfLinks.Count());
               
           
            var converter = new ReverseMarkdown.Converter(config);
            var response = await client.GetAsync(link);
            var pageContents = await response.Content.ReadAsStringAsync();
            pageDocument.LoadHtml(pageContents);

            // Use XPath to find the div with an Id='Content'
            var blogPostContent = pageDocument.DocumentNode.SelectSingleNode("(//div[contains(@id,'content')])").OuterHtml;

            string blogPostMarkdown = converter.Convert(blogPostContent);

            
            // Output to the console. 
            // Console.WriteLine(blogPostMarkdown);


            // Create Markdown file, pass the markdown and id

            CreateMarkDownFile(blogPostMarkdown, id);

            }
            // TODO:
            // Once all files are saved, git push them to a private repo. 
            //

            Console.WriteLine();
            Console.WriteLine("Finished....Press ENTER to exit.");
            Console.ReadLine();
        }

        public static List<string> ReadRssFeedAsync()
        {
            List<string> links = new List<string>();

            // This url could be read in from the console I guess.
            var feedUrl = "https://owain.codes/blog/rss/";
            var readerTask = FeedReader.ReadAsync(feedUrl);
           
            // Get the link from the RSS feed, remove trailing edges and remove any mess e.g. new lines
            // Add it to a list.
            foreach (var item in readerTask.Result.Items)
            {
                
               // Console.WriteLine(item.Link);
                var cleanedUpLink = item.Link.Trim().Replace(@"\t|\n|\r", "");
                links.Add(cleanedUpLink);
            }

            return links;
        }


        public static void CreateMarkDownFile(string blog, int blogNumber)
        {
            string path = @"c:\temp\markdown\blog"+blogNumber+".md";

           
            // Only creates a new file if file doesn't already exist
            if(!File.Exists(path))
            {
                File.AppendAllText(path, blog);
                
            }

        }
    }
}
