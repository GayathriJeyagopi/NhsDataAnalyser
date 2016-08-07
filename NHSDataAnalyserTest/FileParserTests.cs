using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NHSDataAnalyser;
using NUnit.Framework;

namespace NHSDataAnalyserTest
{
    [TestFixture]
    public class FileParserTests
    {
        [Test]
        public void FileParser_ConstructedWithEmptyFileName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new FileParser(string.Empty, false));
        }

        [Test]
        public void FileParser_ConstructedWithNullFileName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new FileParser(null, false));
        }

        [Test]
        public void Parse_FileContainsEmpty_ReturnsEmpty()
        {
            var mockFileWrapper = new Mock<IFileWrapper>();
            mockFileWrapper.Setup(m => m.ReadLines("DummyFileName")).Returns(new List<string>());

            var fileParser = new FileParser("DummyFileName", true, mockFileWrapper.Object);
            var dummyRepository = fileParser.Parse();
            Assert.That(!dummyRepository.Any());
        }

        [Test]
        public void Parse_FileHasHeader_ReturnsSkippingHeaderRemovingExtraSpaces()
        {
            var mockFileWrapper = new Mock<IFileWrapper>();
            mockFileWrapper.Setup(m => m.ReadLines("DummyFileName")).Returns(
                new List<string> {"Header1,Header2,Header3, Header4", "  Value1,Value2, Value3   , Value4"});

            var fileParser = new FileParser("DummyFileName", true, mockFileWrapper.Object);
            var parsedString = fileParser.Parse().ToList();

            Assert.That(parsedString.Count == 1, "There is one list of string, excluding the first line as header");
            var parsedValues = parsedString.First();
            Assert.That(parsedValues.Count == 4, "Four Column values after parsing the comma delimiter");

            Assert.That(parsedValues[0].Equals("Value1"));
            Assert.That(parsedValues[1].Equals("Value2"));
            Assert.That(parsedValues[2].Equals("Value3"));
            Assert.That(parsedValues[3].Equals("Value4"));
        }

        [Test]
        public void Parse_FileHasNoHeader_ReturnsAllValuesRemovingExtraSpaces()
        {
            var mockFileWrapper = new Mock<IFileWrapper>();
            mockFileWrapper.Setup(m => m.ReadLines("DummyFileName")).Returns(
                new List<string> { "Value01       ,Value02,Value03, Value04", "  Value1,Value2, Value3   , Value4" });

            var fileParser = new FileParser("DummyFileName", false, mockFileWrapper.Object);
            var parsedString = fileParser.Parse().ToList();

            Assert.That(parsedString.Count == 2, "There should be 2 parsed string");
            var parsedValues1 = parsedString.First();
            Assert.That(parsedValues1.Count == 4, "Four Column values after parsing the comma delimiter");

            Assert.That(parsedValues1[0].Equals("Value01"));
            Assert.That(parsedValues1[1].Equals("Value02"));
            Assert.That(parsedValues1[2].Equals("Value03"));
            Assert.That(parsedValues1[3].Equals("Value04"));

            var parsedValues2 = parsedString[1];
            Assert.That(parsedValues2.Count == 4, "Four Column values after parsing the comma delimiter");

            Assert.That(parsedValues2[0].Equals("Value1"));
            Assert.That(parsedValues2[1].Equals("Value2"));
            Assert.That(parsedValues2[2].Equals("Value3"));
            Assert.That(parsedValues2[3].Equals("Value4"));

        }
    }
}