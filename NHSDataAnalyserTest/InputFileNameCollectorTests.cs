using System;
using Moq;
using NHSDataAnalyser;
using NUnit.Framework;

namespace NHSDataAnalyserTest
{
    [TestFixture]
    public class InputFileNameCollectorTests
    {
        private const string DataFor = "SomeName";
        private const string SomeFileName = "SomeFileName";
        private Mock<IFileWrapper> _mockFileWrapper;
        private Mock<IConsoleWrapper> _mockConsoleWrapper;

        [SetUp]
        public void SetUp()
        {
            _mockFileWrapper = new Mock<IFileWrapper>();
            _mockConsoleWrapper = new Mock<IConsoleWrapper>();
        }

        [TearDown]
        public void TearDown()
        {
            _mockFileWrapper = null;
            _mockConsoleWrapper = null;
        }

        [Test]
        public void Collect_FileDoesnotExists_ReturnsEmpty()
        {
            _mockConsoleWrapper.Setup(m => m.ReadLine()).Returns(SomeFileName);
            _mockFileWrapper.Setup(m => m.Exists(It.IsAny<string>())).Returns(false);

            var inputFileNameCollector = new InputFileNameCollector(_mockConsoleWrapper.Object, _mockFileWrapper.Object);
            Assert.IsNullOrEmpty(inputFileNameCollector.Collect(DataFor), "The file does not exist");
        }

        [Test]
        public void Collect_FileExists_ReturnsFileName()
        {
            _mockConsoleWrapper.Setup(m => m.ReadLine()).Returns(SomeFileName);
            SetUpFileExist(true);

            var inputFileNameCollector = new InputFileNameCollector(_mockConsoleWrapper.Object, _mockFileWrapper.Object);

            Assert.AreEqual(inputFileNameCollector.Collect(DataFor), SomeFileName);
        }

        [Test]
        public void Collect_FileDoesnotExistsAndEnterKeyPressed_ConsoleIsReadAgain()
        {
            _mockConsoleWrapper.Setup(m => m.ReadLine()).Returns(SomeFileName);

            SetUpFileExist(false);

            var consoleKeyInfo = new ConsoleKeyInfo('c', ConsoleKey.Enter, false, false, false);
            _mockConsoleWrapper.Setup(m => m.ReadKey()).Returns(consoleKeyInfo).Callback(() => SetUpFileExist(true));
           
            var inputFileNameCollector = new InputFileNameCollector(_mockConsoleWrapper.Object, _mockFileWrapper.Object);
            inputFileNameCollector.Collect(DataFor);
    
            _mockConsoleWrapper.Verify(m => m.ReadLine(), Times.Exactly(2));
        }

        [Test]
        public void Collect_FileDoesnotExistsAndSomeKeyPressed_ConsoleIsNotRead()
        {
            _mockConsoleWrapper.Setup(m => m.ReadLine()).Returns(SomeFileName);

            SetUpFileExist(false);

            var consoleKeyInfo = new ConsoleKeyInfo('c', ConsoleKey.A, false, false, false);
            _mockConsoleWrapper.Setup(m => m.ReadKey()).Returns(consoleKeyInfo);

            var inputFileNameCollector = new InputFileNameCollector(_mockConsoleWrapper.Object, _mockFileWrapper.Object);
            inputFileNameCollector.Collect(DataFor);

            _mockConsoleWrapper.Verify(m => m.ReadLine(), Times.Exactly(1));
        }

        private void SetUpFileExist(bool isExist)
        {
            _mockFileWrapper.Setup(m => m.Exists(It.IsAny<string>())).Returns(isExist);
        }
    }
}