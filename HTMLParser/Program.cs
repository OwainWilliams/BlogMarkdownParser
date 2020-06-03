using CodeHollow.FeedReader;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
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


            // WIP.
            foreach (var link in listOfLinks)
            {
                var temp = link;
            }



            // GetAsync will be the Link returned from the RSS feed.
            var converter = new ReverseMarkdown.Converter(config);
            var response = await client.GetAsync("https://owain.codes/blog/posts/2020/march/umbraco-mvp-heather-floyd/");
            var pageContents = await response.Content.ReadAsStringAsync();


            pageDocument.LoadHtml(pageContents);

            var blogPostContent = pageDocument.DocumentNode.SelectSingleNode("(//div[contains(@id,'content')])").OuterHtml;


            string html = blogPostContent;
            string blogPostMarkdown = converter.Convert(html);


            //Instead of outputting to the console. Save as Markdown file. 
            Console.WriteLine(blogPostMarkdown);
            Console.ReadLine();

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
                Console.WriteLine(item.Title + " - " + item.Link);
            }



            return links;
        }
    }
}
