using Microsoft.VisualStudio.TestTools.UnitTesting;
using HTMLParser;
using HTMLParser.Processing.Processors;


namespace HTMLParserTests
{
    [TestClass]
    public class HTMLParserTests


    {

        // This allows me to output the actual results in to the Test Detail Summary.

        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }

        }



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


        [TestMethod]
        public void Get_Filename_From_Url()
        {
            // Arrange
            string url = "https://owain.codes/something/";
            string expected = "something";

            // Act
            HtmlProcessor test = new HtmlProcessor(url);


            // Assert
            string actual = test.GetFilename(url);
            Assert.AreEqual(expected, actual);
            TestContext.WriteLine(actual);
        }
    }
}
