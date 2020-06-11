using Microsoft.VisualStudio.TestTools.UnitTesting;
using HTMLParser;
using HTMLParser.Processing.Processors;

namespace HTMLParserTests
{
    [TestClass]
    public class HTMLParserTests
    {
        [TestMethod]
        public void Remove_Whitespace_From_End_Url()
        {
            // Arrange
            string url = "https://owain.codes/something ";
            string expected = "https://owain.codes/something";

            // Act
            RssFeedProcessor test = new RssFeedProcessor(url);


            // Assert
            string actual = test.CleanupLink(url);
            Assert.AreEqual(expected, actual, "URL hasn't been cleaned up");
        }
        [TestMethod]
        public void Remove_Whitespace_From_Start_Url()
        {
            // Arrange
            string url = " https://owain.codes/something";
            string expected = "https://owain.codes/something";

            // Act
            RssFeedProcessor test = new RssFeedProcessor(url);


            // Assert
            string actual = test.CleanupLink(url);
            Assert.AreEqual(expected, actual, "URL hasn't been cleaned up");
        }
    }
}
