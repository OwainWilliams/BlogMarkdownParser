using System.Collections.Generic;
using System.Threading.Tasks;
using CodeHollow.FeedReader;

namespace HTMLParser.Processing.Processors
{
    public class RssFeedProcessor
    {
        private readonly string FeedUrl;

        public RssFeedProcessor(string feedUrl)
        {
            this.FeedUrl = feedUrl;
        }
        
        public async Task<List<string>> GetFeedLinksAsync()
        {
            List<string> links = new List<string>();
            
            var readerTask = await FeedReader.ReadAsync(FeedUrl);

            // Get the link from the RSS feed, remove trailing edges and remove any mess e.g. new lines
            // Add it to a list.
            foreach (var item in readerTask.Items)
            {
                var cleanedUpLink = item.Link.Trim().Replace(@"\t|\n|\r", "");
                links.Add(cleanedUpLink);
            }

            return links;
        }
    }
}