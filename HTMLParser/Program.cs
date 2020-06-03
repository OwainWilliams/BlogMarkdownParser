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

            int id = 0;
            // WIP.
            foreach (var link in listOfLinks)
            {

                id++;
            // GetAsync will be the Link returned from the RSS feed.
            var converter = new ReverseMarkdown.Converter(config);
            var response = await client.GetAsync(link);
            var pageContents = await response.Content.ReadAsStringAsync();


            pageDocument.LoadHtml(pageContents);

            var blogPostContent = pageDocument.DocumentNode.SelectSingleNode("(//div[contains(@id,'content')])").OuterHtml;


            string html = blogPostContent;
            string blogPostMarkdown = converter.Convert(html);


            //Instead of outputting to the console. Save as Markdown file. 
            Console.WriteLine(blogPostMarkdown);

                

            CreateMarkDownFile(blogPostMarkdown, id);


            }
            // TODO:
            // Once all files are saved, git push them to a private repo. 
            //
        }

        public static List<string> ReadRssFeedAsync()
        {
            List<string> links = new List<string>();

            var feedUrl = "https://owain.codes/blog/rss/";
           

            var readerTask = FeedReader.ReadAsync(feedUrl);
           

            foreach (var item in readerTask.Result.Items)
            {
                Console.WriteLine(item.Link);
                var cleanedUpLink = item.Link.Trim().Replace(@"\t|\n|\r", "");

                links.Add(cleanedUpLink);
            }



            return links;
        }


        public static void CreateMarkDownFile(string blog, int blogNumber)
        {
            string path = @"c:\temp\markdown\blog"+blogNumber+".md";
            if(!File.Exists(path))
            {
                File.AppendAllText(path, blog);
                Console.WriteLine("Created Blog: " + blogNumber);
            }


        }
    }
}
