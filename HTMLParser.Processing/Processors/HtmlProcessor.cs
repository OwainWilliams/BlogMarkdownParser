using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Serilog;
using SixLabors.ImageSharp;

namespace HTMLParser.Processing.Processors
{
    public class HtmlProcessor
    {

        public string folderDirectory;

        private HttpClient client;
        private WebClient webClient;
        private HtmlDocument pageHTML;




        private static readonly ReverseMarkdown.Config MdConfig = new ReverseMarkdown.Config
        {
            UnknownTags = ReverseMarkdown.Config.UnknownTagsOption.Bypass, // Include the unknown tag completely in the result (default as well)
            GithubFlavored = true, // generate GitHub flavoured markdown, supported for BR, PRE and table tags
            RemoveComments = true, // will ignore all comments
            SmartHrefHandling = true // remove markdown output for links where appropriate
        };

        private string BlogPath { get; set; }
        private string Domain { get; set; }

        public HtmlProcessor(string blogPath, string domain)
        {
            this.client = new HttpClient();
            this.webClient = new WebClient();
            this.BlogPath = blogPath;
            this.Domain = domain;
        }

        public async Task ProcessLinks(List<string> listOfLinks)
        {
            this.pageHTML = new HtmlDocument();

            // Configure how to handle the HTML and how to parse it.
            for (var i = 1; i <= listOfLinks.Count; i += 2)
            {
                var link = listOfLinks[i - 1];
                // TODO: replace with a logger so not specific to a console app
                var date = listOfLinks[i];

                var converter = new ReverseMarkdown.Converter(MdConfig);
                var response = await client.GetAsync(link);
                var pageContents = await response.Content.ReadAsStringAsync();
                pageHTML.LoadHtml(pageContents);

                // Use XPath to find the div with an Id='Content'
                //var blogPostContent = pageHTML.DocumentNode.SelectSingleNode("(//div[contains(@id,'content' and not (@class='featured-blogs'))]").OuterHtml;

                var blogPostContent = pageHTML.DocumentNode.SelectSingleNode("(//*[@id='content' and not(@class='slick-track')])").InnerHtml;

                

                string blogPostMarkdown = converter.Convert(blogPostContent);

                //Trim Url to get filename
                string filename = this.GetFilename(link);


                // Create Markdown file, pass the markdown, filename and id
                this.CreateMarkDownFile(blogPostMarkdown, filename, date);

                // Get any images that are in the blog
                this.GetAllImagesFromBlog(blogPostContent, folderDirectory);
            }
        }

        private void CreateMarkDownFile(string blog, string filename, string date)
        {


            folderDirectory = $@"{this.BlogPath}{date}-{filename}";
            string directoryAndFilename = folderDirectory + "\\" + filename + ".md";

            try
            {
                // Check if Directory already exists
                if (Directory.Exists(folderDirectory))
                {
                    // If the file doesn't already exist, create it
                    if (!File.Exists(filename))
                    {
                        File.AppendAllText(directoryAndFilename, blog);
                    }
                }
                else
                {

                    DirectoryInfo di = Directory.CreateDirectory(folderDirectory);
                    File.AppendAllText(directoryAndFilename, blog);

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex);
            }
            finally { }

        }

        private void GetAllImagesFromBlog(string blog, string folderDirectory)
        {

            // BUG: Need to find a way to handle 404 errors, currently this it breaking. 

            HtmlDocument document = new HtmlDocument();
            List<string> imageLinks = new List<string>();
            document.LoadHtml(blog);


            HtmlNodeCollection nodes = document.DocumentNode.SelectNodes("//img");

            int i = 0;
            if (nodes != null)
            {
                foreach (HtmlNode link in nodes)
                {
                   
                   
                    imageLinks.Add(link.GetAttributeValue("src", ""));
                    string imgFilePath = imageLinks[i].ToString();

                    // Ignore any files that are outwith local
                    if (imgFilePath.Contains("http"))
                    {
                        continue;
                    }

                    string imageFileName = GetFilename(imgFilePath);
                    string imageFilePathWithoutCrop = GetFilePathWithoutCrops(imgFilePath);
                    

                    Uri uri = new Uri(this.Domain + imageFilePathWithoutCrop);
                    try
                    {
                        HttpWebRequest myHttpRequest = (HttpWebRequest)WebRequest.Create(uri);
                        HttpWebResponse myHttpResponse = (HttpWebResponse)myHttpRequest.GetResponse();

                        Log.Information("Getting this file : " + uri);
                        webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback);

                        var task = webClient.DownloadFileTaskAsync(uri, folderDirectory + "\\" + imageFileName);
                        task.Wait();

                        Log.Information("Download complete");
                        Console.WriteLine("Download complete");
                        i++;
                        myHttpResponse.Close();

                        ImageProcessor processImage = new ImageProcessor();
                        processImage.ResizeImage(imageFileName, folderDirectory);


                    }
                    catch(WebException e)
                    {
                        Log.Information("FILE NOT FOUND " + uri);
                        Log.Information("Something went wrong with " + e.Message);
                        i++;
                        continue;
                    }
                   


                }
              
            }
        }

        public string GetFilename(string blogUrl)
        {
            string removeTrailingSlash = blogUrl.TrimEnd('/');
            string fileName = removeTrailingSlash.Substring(removeTrailingSlash.LastIndexOf("/") + 1);
            if (fileName.Contains("?"))
            {
                fileName = fileName.Substring(0, fileName.IndexOf("?"));
            }


            return fileName;
        }

        public string GetFilePathWithoutCrops(string imageFilePath)
        {
            if (imageFilePath.Contains("?"))
            {
                imageFilePath = imageFilePath.Substring(0, imageFilePath.IndexOf("?"));
            }
            return imageFilePath;
        }

        private static void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
        {

       

            // Displays the operation identifier, and the transfer progress.
            Console.WriteLine("{0}    downloaded {1} of {2} Mb. {3} % complete...",
                ((TaskCompletionSource<object>)e.UserState).Task.AsyncState,
                (e.BytesReceived / 1024d / 1024d).ToString("0.00"),
                (e.TotalBytesToReceive / 1024d / 1024d).ToString("0.00"),
                e.ProgressPercentage);
        }



    }
}