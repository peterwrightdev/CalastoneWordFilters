using CalastoneWordFilterer;
using CalastoneWordFilterer.Factories;
using CalastoneWordFilterer.FileReaders;
using CalastoneWordFilterer.Consoles;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CalastoneWordFilterer.Filters;
using System.Reflection;

namespace WordFiltererTestProject
{
    [TestClass]
    public class ClientTester
    {
        [TestMethod]
        public void RequestFileFromUser_WhenFileIsFound_ThenReturnedStreamReaderPointsToTheFile()
        {
            // Arrange
            Mock<IFilterFactory> moqFilterFactory = new Mock<IFilterFactory>();
            Mock<IFileReader> moqFileReader = new Mock<IFileReader>();
            string moqFileContext = "This is the mock content of my file.";
            byte[] fakeFileBytes = Encoding.UTF8.GetBytes(moqFileContext);
            MemoryStream fakeMemoryStream = new MemoryStream(fakeFileBytes);

            Mock<IConsole> moqConsole = new Mock<IConsole>();
            moqConsole.SetupSequence(m => m.ReadLine()).Returns("mockfilename.txt");

            moqFileReader.Setup(mfr => mfr.GetStreamReader(It.Is<string>(s => s == "mockfilename.txt"))).Returns(() => new StreamReader(fakeMemoryStream));
            Client core = new Client(moqFilterFactory.Object, moqFileReader.Object, moqConsole.Object);

            // Act
            StreamReader outputStreamReader = core.RequestFileFromUser();

            // Assert
            // verify that correct messages were sent to the user.
            moqConsole.Verify(c => c.WriteLine(It.Is<string>(s => s == "Please supply the filename to filter.")), Times.Once);
            
            // verify that the streamreader returned is pointing to our "file" which contains content as we defined.
            string outputFileContent = outputStreamReader.ReadToEnd();
            Assert.AreEqual(moqFileContext, outputFileContent);
        }

        [TestMethod]
        public void RequestFileFromUser_WhenFileIsNotFound_ThenErrorMessageIsReturnedToUserAndNotFoundExceptionIsThrown()
        {
            // Arrange
            Mock<IFilterFactory> moqFilterFactory = new Mock<IFilterFactory>();
            Mock<IFileReader> moqFileReader = new Mock<IFileReader>();

            Mock<IConsole> moqConsole = new Mock<IConsole>();
            moqConsole.SetupSequence(m => m.ReadLine()).Returns("mockfilename.txt");

            moqFileReader.Setup(mfr => mfr.GetStreamReader(It.Is<string>(s => s == "mockfilename.txt"))).Throws(new Exception());
            Client core = new Client(moqFilterFactory.Object, moqFileReader.Object, moqConsole.Object);


            Exception expectedException = null;
            try
            {
                // Act
                core.RequestFileFromUser();
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            // Assert
            // verify that correct messages were sent to the user.
            moqConsole.Verify(c => c.WriteLine(It.Is<string>(s => s == "Please supply the filename to filter.")), Times.Once);
            moqConsole.Verify(c => c.WriteLine(It.Is<string>(s => s == "Failed to find file mockfilename.txt. Please ensure path is correct.")), Times.Once);

            Assert.AreEqual(typeof(FileNotFoundException), expectedException.GetType());
        }

        [TestMethod]
        public void GetFiltersToApply_WhenOneParameterlessFilterExists_ThenUserChoosesToApplyIt_ThenFilterIsReturned()
        {
            // Arrange
            Mock<IFilter> moqFilter = new Mock<IFilter>();

            Mock<IFilterFactory> moqFilterFactory = new Mock<IFilterFactory>();
            moqFilterFactory.Setup(mff => mff.GetFilterTypes()).Returns(new List<Type>() { typeof(ExcludeShortWordsFilter) });
            moqFilterFactory.Setup(mff => mff.CreateFilter(typeof(ExcludeShortWordsFilter), null)).Returns(moqFilter.Object);

            Mock<IFileReader> moqFileReader = new Mock<IFileReader>();

            Mock<IConsole> moqConsole = new Mock<IConsole>();
            moqConsole.SetupSequence(m => m.ReadLine()).Returns("0").Returns(string.Empty);

            Client core = new Client(moqFilterFactory.Object, moqFileReader.Object, moqConsole.Object);

            // Act
            List<IFilter> outputFiltersToApply = core.GetFiltersToApply();

            // Assert
            // verify that correct messages were sent to the user.
            moqConsole.Verify(c => c.WriteLine(It.Is<string>(s => s == "Choose filters to apply from the following;")), Times.Once);
            moqConsole.Verify(c => c.WriteLine(It.Is<string>(s => s == "0: ExcludeShortWordsFilter")), Times.Once);
            moqConsole.Verify(c => c.WriteLine(It.Is<string>(s => s == "For example, type '0 1' to apply the first two filters only.")), Times.Once);
            
            // verify our mock filter was returned - thereby ensuring that the Core class used our factory to create a filter of type ExcludeShortWordsFilter
            Assert.AreEqual(1, outputFiltersToApply.Count);
            Assert.AreEqual(moqFilter.Object, outputFiltersToApply[0]);
        }

        [TestMethod]
        public void GetFiltersToApply_WhenUserMakesInvalidSelection_ThenErrorMessageIsReturned_AndExceptionIsThrown()
        {
            // Arrange
            Mock<IFilter> moqFilter = new Mock<IFilter>();

            Mock<IFilterFactory> moqFilterFactory = new Mock<IFilterFactory>();
            moqFilterFactory.Setup(mff => mff.GetFilterTypes()).Returns(new List<Type>() { typeof(ExcludeShortWordsFilter) });
            moqFilterFactory.Setup(mff => mff.CreateFilter(typeof(ExcludeShortWordsFilter), null)).Returns(moqFilter.Object);

            Mock<IFileReader> moqFileReader = new Mock<IFileReader>();

            Mock<IConsole> moqConsole = new Mock<IConsole>();
            moqConsole.SetupSequence(m => m.ReadLine()).Returns("invalidselection").Returns(string.Empty);

            Client core = new Client(moqFilterFactory.Object, moqFileReader.Object, moqConsole.Object);

            // Act
            Exception exception = null;
            try
            {
                core.GetFiltersToApply();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Assert
            // verify that correct messages were sent to the user.
            moqConsole.Verify(c => c.WriteLine(It.Is<string>(s => s == "Choose filters to apply from the following;")), Times.Once);
            moqConsole.Verify(c => c.WriteLine(It.Is<string>(s => s == "0: ExcludeShortWordsFilter")), Times.Once);
            moqConsole.Verify(c => c.WriteLine(It.Is<string>(s => s == "For example, type '0 1' to apply the first two filters only.")), Times.Once);
            moqConsole.Verify(c => c.WriteLine(It.Is<string>(s => s == "Failed to apply filters.")), Times.Once);

            Assert.AreEqual(typeof(ApplicationException), exception.GetType());
        }

        [TestMethod]
        public void GetFiltersToApply_WhenOneParameterlessFilterExists_ThenUserChoosesNotToApplyIt_ThenFilterIsNotReturned()
        {
            // Arrange
            Mock<IFilter> moqFilter = new Mock<IFilter>();

            Mock<IFilterFactory> moqFilterFactory = new Mock<IFilterFactory>();
            moqFilterFactory.Setup(mff => mff.GetFilterTypes()).Returns(new List<Type>() { typeof(ExcludeShortWordsFilter) });
            moqFilterFactory.Setup(mff => mff.CreateFilter(typeof(ExcludeShortWordsFilter), null)).Returns(moqFilter.Object);

            Mock<IFileReader> moqFileReader = new Mock<IFileReader>();

            Mock<IConsole> moqConsole = new Mock<IConsole>();
            moqConsole.SetupSequence(m => m.ReadLine()).Returns(string.Empty).Returns(string.Empty);

            Client core = new Client(moqFilterFactory.Object, moqFileReader.Object, moqConsole.Object);

            // Act
            List<IFilter> outputFiltersToApply = core.GetFiltersToApply();

            // Assert
            // verify that correct messages were sent to the user.
            moqConsole.Verify(c => c.WriteLine(It.Is<string>(s => s == "Choose filters to apply from the following;")), Times.Once);
            moqConsole.Verify(c => c.WriteLine(It.Is<string>(s => s == "0: ExcludeShortWordsFilter")), Times.Once);
            moqConsole.Verify(c => c.WriteLine(It.Is<string>(s => s == "For example, type '0 1' to apply the first two filters only.")), Times.Once);

            Assert.AreEqual(0, outputFiltersToApply.Count);
        }

        [TestMethod]
        public void GetFiltersToApply_WhenOneParameterisedFilterExists_ThenUserChoosesToApplyIt_ThenUserChoosesToUseDefaultValue_ThenFilterWithDefaultValueIsReturned()
        {
            // Arrange
            Mock<IFilter> moqFilter = new Mock<IFilter>();

            Mock<IFilterFactory> moqFilterFactory = new Mock<IFilterFactory>();
            moqFilterFactory.Setup(mff => mff.GetFilterTypes()).Returns(new List<Type>() { typeof(ExcludeShortWordsFilter) });
            moqFilterFactory.Setup(mff => mff.CreateFilter(typeof(ExcludeShortWordsFilter), new object[] { 3 })).Returns(moqFilter.Object);

            ParameterInfo[] moqParameters = new ParameterInfo[1];
            Mock<ParameterInfo> mockParamInfo = new Mock<ParameterInfo>();

            // mockParamInfo.Setup(mpi => mpi.IsOptional).Returns(true);
            mockParamInfo.Setup(mpi => mpi.Name).Returns("mockparam");
            mockParamInfo.Setup(mpi => mpi.ParameterType).Returns(typeof(int));
            mockParamInfo.Setup(mpi => mpi.DefaultValue).Returns(3);
            moqParameters[0] = mockParamInfo.Object;
            moqFilterFactory.Setup(mff => mff.GetParametersForFilter(It.Is<Type>(t => t == typeof(ExcludeShortWordsFilter)))).Returns(moqParameters);

            Mock<IFileReader> moqFileReader = new Mock<IFileReader>();

            Mock<IConsole> moqConsole = new Mock<IConsole>();
            moqConsole.SetupSequence(m => m.ReadLine()).Returns("0").Returns(string.Empty);

            Client core = new Client(moqFilterFactory.Object, moqFileReader.Object, moqConsole.Object);

            // Act
            List<IFilter> outputFiltersToApply = core.GetFiltersToApply();

            // Assert
            // verify that correct messages were sent to the user.
            moqConsole.Verify(c => c.WriteLine(It.Is<string>(s => s == "Choose filters to apply from the following;")), Times.Once);
            moqConsole.Verify(c => c.WriteLine(It.Is<string>(s => s == "0: ExcludeShortWordsFilter")), Times.Once);
            moqConsole.Verify(c => c.WriteLine(It.Is<string>(s => s == "For example, type '0 1' to apply the first two filters only.")), Times.Once);

            moqConsole.Verify(c => c.WriteLine(It.Is<string>(s => s == "Filter ExcludeShortWordsFilter has required parameter mockparam. Please enter a Int32")), Times.Once);

            // verify our mock filter was returned - thereby ensuring that the Core class used our factory to create a filter of type ExcludeShortWordsFilter
            Assert.AreEqual(1, outputFiltersToApply.Count);
            Assert.AreEqual(moqFilter.Object, outputFiltersToApply[0]);
        }

        [TestMethod]
        public void GetFiltersToApply_WhenOneParameterisedFilterExists_ThenUserChoosesToApplyIt_ThenUserChoosesToUseNonDefaultValue_ThenFilterWithNonDefaultValueIsReturned()
        {
            // Arrange
            Mock<IFilter> moqFilter = new Mock<IFilter>();

            Mock<IFilterFactory> moqFilterFactory = new Mock<IFilterFactory>();
            moqFilterFactory.Setup(mff => mff.GetFilterTypes()).Returns(new List<Type>() { typeof(ExcludeShortWordsFilter) });
            moqFilterFactory.Setup(mff => mff.CreateFilter(typeof(ExcludeShortWordsFilter), new object[] { 6 })).Returns(moqFilter.Object);

            ParameterInfo[] moqParameters = new ParameterInfo[1];
            Mock<ParameterInfo> mockParamInfo = new Mock<ParameterInfo>();

            // mockParamInfo.Setup(mpi => mpi.IsOptional).Returns(true);
            mockParamInfo.Setup(mpi => mpi.Name).Returns("mockparam");
            mockParamInfo.Setup(mpi => mpi.ParameterType).Returns(typeof(int));
            mockParamInfo.Setup(mpi => mpi.DefaultValue).Returns(3);
            moqParameters[0] = mockParamInfo.Object;
            moqFilterFactory.Setup(mff => mff.GetParametersForFilter(It.Is<Type>(t => t == typeof(ExcludeShortWordsFilter)))).Returns(moqParameters);

            Mock<IFileReader> moqFileReader = new Mock<IFileReader>();

            Mock<IConsole> moqConsole = new Mock<IConsole>();
            moqConsole.SetupSequence(m => m.ReadLine()).Returns("0").Returns("6");

            Client core = new Client(moqFilterFactory.Object, moqFileReader.Object, moqConsole.Object);

            // Act
            List<IFilter> outputFiltersToApply = core.GetFiltersToApply();

            // Assert
            // verify that correct messages were sent to the user.
            moqConsole.Verify(c => c.WriteLine(It.Is<string>(s => s == "Choose filters to apply from the following;")), Times.Once);
            moqConsole.Verify(c => c.WriteLine(It.Is<string>(s => s == "0: ExcludeShortWordsFilter")), Times.Once);
            moqConsole.Verify(c => c.WriteLine(It.Is<string>(s => s == "For example, type '0 1' to apply the first two filters only.")), Times.Once);

            // verify our mock filter was returned - thereby ensuring that the Core class used our factory to create a filter of type ExcludeShortWordsFilter
            Assert.AreEqual(1, outputFiltersToApply.Count);
            Assert.AreEqual(moqFilter.Object, outputFiltersToApply[0]);
        }

        [TestMethod]
        public void PrintFilteredWordsFromStream_MethodAppliesFiltersOntoStreamAndReturnsResult()
        {
            // Arrange
            Mock<IConsole> moqConsole = new Mock<IConsole>();
            Mock<IFilterFactory> moqFilterFactory = new Mock<IFilterFactory>();
            Mock<IFileReader> moqFileReader = new Mock<IFileReader>();

            Mock<IFilter> mockFilter = new Mock<IFilter>();
            mockFilter.Setup(mf => mf.ExcludeWord(It.Is<string>(s => s.Contains("o")))).Returns(true);
            mockFilter.Setup(mf => mf.ExcludeWord(It.Is<string>(s => !s.Contains("o")))).Returns(false);

            List<IFilter> filtersToApply = new List<IFilter>() { mockFilter.Object };

            string moqFileContext = "This is the mock content of my file.";
            byte[] fakeFileBytes = Encoding.UTF8.GetBytes(moqFileContext);
            MemoryStream fakeMemoryStream = new MemoryStream(fakeFileBytes);

            Client core = new Client(moqFilterFactory.Object, moqFileReader.Object, moqConsole.Object);
            core.PrintFilteredWordsFromStream(new StreamReader(fakeMemoryStream), filtersToApply);

            moqConsole.Verify(c => c.WriteLine(It.Is<string>(s => s == "Result is as follows;")), Times.Once);
            moqConsole.Verify(c => c.WriteLine(It.Is<string>(s => s == "This is the my file")), Times.Once);
        }
    }
}
