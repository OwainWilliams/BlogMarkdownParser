using HtmlAgilityPack;
using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;
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
                        
            var listOfUrls = await ReadRssFeedAsync();
            
            
            
            
            HttpClient client = new HttpClient();
            HtmlDocument pageDocument = new HtmlDocument();

            var config = new ReverseMarkdown.Config
            {
                UnknownTags = ReverseMarkdown.Config.UnknownTagsOption.Bypass, // Include the unknown tag completely in the result (default as well)
                GithubFlavored = true, // generate GitHub flavoured markdown, supported for BR, PRE and table tags
                RemoveComments = true, // will ignore all comments
                SmartHrefHandling = true // remove markdown output for links where appropriate
            };


            foreach (var link in listOfUrls)
            {
                var temp = link;
            }


            var converter = new ReverseMarkdown.Converter(config);

            var response = await client.GetAsync("https://owain.codes/blog/posts/2020/march/umbraco-mvp-heather-floyd/");
            var pageContents = await response.Content.ReadAsStringAsync();


            pageDocument.LoadHtml(pageContents);

            var blogPostContent = pageDocument.DocumentNode.SelectSingleNode("(//div[contains(@id,'content')])").OuterHtml;


            string html = blogPostContent;
            string blogPostMarkdown = converter.Convert(html);

            Console.WriteLine(blogPostMarkdown);
            Console.ReadLine();
        }

        public static async Task<List<string>> ReadRssFeedAsync()
        {
            var links = new List<string>();

            string url = "https://owain.codes/blog/rss/";
            using (var xmlReader = XmlReader.Create(url, new XmlReaderSettings() { Async = true }))
            {
                var feedReader = new RssFeedReader(xmlReader);

                while (await feedReader.Read())
                {
                    switch (feedReader.ElementType)
                    {
                        // Read category
                        case SyndicationElementType.Category:
                            ISyndicationCategory category = await feedReader.ReadCategory();
                            break;

                        // Read Image
                        case SyndicationElementType.Image:
                            ISyndicationImage image = await feedReader.ReadImage();
                            break;

                        // Read Item
                        case SyndicationElementType.Item:
                            ISyndicationItem item = await feedReader.ReadItem();
                            if(item.Id !=null)
                            {
                                links.Add(item.Id);
                            }
                            
                                
                            break;

                        // Read link
                        case SyndicationElementType.Link:
                            ISyndicationLink link = await feedReader.ReadLink();
                    
                            break;

                        // Read Person
                        case SyndicationElementType.Person:
                            ISyndicationPerson person = await feedReader.ReadPerson();
                            break;

                        // Read content
                        default:
                            ISyndicationContent content = await feedReader.ReadContent();
                            break;
                    }
                }

               
            }

            return links;
        }
    }
}
