using Microsoft.VisualStudio.TestTools.UnitTesting;
using HTMLParser.Processing.Processors;
using System;

namespace HTMLParserTests
{
    [TestClass]
    public class ImageProcessorTests
    {
        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }

        }

        [TestMethod]
        public void Resize_Image()
        {
            // Arrange
            string filePath = "https://owain.codes/something ";
            string expected = "https://owain.codes/something";

            // Act
            ImageProcessor imageProcessor = new ImageProcessor(filePath);


            // Assert
            string actual = "";
            Assert.AreEqual(expected, actual, "URL hasn't been cleaned up");
        }


    }
}