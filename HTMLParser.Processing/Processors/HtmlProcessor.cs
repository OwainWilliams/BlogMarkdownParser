using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace HTMLParser.Processing.Processors
{
    public class HtmlProcessor
    {
        private HttpClient client;
        private HtmlDocument pageHTML;
        private static readonly ReverseMarkdown.Config MdConfig = new ReverseMarkdown.Config
        {
            UnknownTags = ReverseMarkdown.Config.UnknownTagsOption.Bypass, // Include the unknown tag completely in the result (default as well)
            GithubFlavored = true, // generate GitHub flavoured markdown, supported for BR, PRE and table tags
            RemoveComments = true, // will ignore all comments
            SmartHrefHandling = true // remove markdown output for links where appropriate
        };
        
        private string BlogPath { get; set; }
        
        public HtmlProcessor(string blogPath)
        {
            this.client = new HttpClient();
            this.BlogPath = blogPath;
        }
        
        public async Task ProcessLinks(List<string> listOfLinks)
        {
            this.pageHTML = new HtmlDocument();
            
            // Configure how to handle the HTML and how to parse it.
            for(var i=1; i <= listOfLinks.Count; i++)
            {
                var link = listOfLinks[i-1];
                //TODO: replace with a logger so not specific to a console app
                Console.Write($"\rImporting {i} of {listOfLinks.Count} - ");
                
                var converter = new ReverseMarkdown.Converter(MdConfig);
                var response = await client.GetAsync(link);
                var pageContents = await response.Content.ReadAsStringAsync();
                pageHTML.LoadHtml(pageContents);

                // Use XPath to find the div with an Id='Content'
                var blogPostContent = pageHTML.DocumentNode.SelectSingleNode("(//div[contains(@id,'content')])").OuterHtml;
                


                string blogPostMarkdown = converter.Convert(blogPostContent);

                //Trim Url to get filename
                string filename = this.GetFilename(link);


                // Create Markdown file, pass the markdown, filename and id
                this.CreateMarkDownFile(blogPostMarkdown, filename, i);

              
            //    this.GetAllImageUrls(blogPostContent);
            }
        }
        
        private void CreateMarkDownFile(string blog, string filename, int blogNumber)
        {


            string path = $@"{this.BlogPath}{filename}.md";
            // Only creates a new file if file doesn't already exist
            if (!File.Exists(path))
                File.AppendAllText(path, blog);
        }

        private void GetAllImageUrls(string blog)
        {
            HtmlAgilityPack.HtmlDocument document = new HtmlDocument();
            List<string> imageLinks = new List<string>();
            document.LoadHtml(blog);

            int i = 0;

            foreach (HtmlNode link in document.DocumentNode.SelectNodes("//img"))
            {
                imageLinks.Add(link.GetAttributeValue("src", ""));
                //string filePath = @"C:\temp\markdown\";
                //string downloadLink = @"https://owain.codes/"+imageLinks[i];

                //using (var imgClient = new WebClient())
                //{
                //    imgClient.DownloadFile(downloadLink, filePath+"image"+i);
                //}
                //i++;
            }
        }

        public string GetFilename(string blogUrl)
        {
            string removeTrailingSlash = blogUrl.TrimEnd('/');
            string fileName = removeTrailingSlash.Substring(removeTrailingSlash.LastIndexOf("/") + 1);
            return fileName;
        }
    }
}