using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieRating;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MovieRating.Models;

namespace MovieRatingTests
{
    [TestClass]
    public class MovieRatingAppTests
    {
        private MovieRatingApp movieRatingApp;
        private string filePath;

        [TestInitialize]
        public void Setup()
        {
            filePath = "C:\\filePathTest.txt";
            movieRatingApp = new MovieRatingApp(filePath);
        }

        #region Helper methods
        
        private void ResetFile()
        {
            string[] fileContent = new string[20];
            fileContent[0] = "1::1193::5::978300760";
            fileContent[1] = "1::914::4::978301968";
            fileContent[2] = "1::913::1::978301969";
            fileContent[3] = "1::912::4::978301970";
            fileContent[4] = "1::911::3::978301971";
            fileContent[5] = "1::910::2::978301972";
            fileContent[6] = "1::909::1::978301973";
            fileContent[7] = "1::908::5::978301974";
            fileContent[8] = "1::907::3::978301975";
            fileContent[9] = "1::906::2::978301976";
            fileContent[10] = "2::1193::5::978301977";
            fileContent[11] = "2::914::3::978301978";
            fileContent[12] = "2::913::1::978301979";
            fileContent[13] = "2::912::1::978301980";
            fileContent[14] = "2::911::2::978301981";
            fileContent[15] = "2::910::3::978301982";
            fileContent[16] = "2::909::4::978301983";
            fileContent[17] = "2::908::1::978301984";
            fileContent[18] = "2::907::2::978301985";
            fileContent[19] = "2::906::1::978301986";

            if (File.Exists(this.filePath))
            {
                File.Delete(this.filePath);
            }

            File.Create(this.filePath).Close();
            File.WriteAllLines(this.filePath, fileContent);
        }

        #endregion

        #region ImportData tests

        [TestMethod]
        public void ImportData_PerfectFile()
        {
            ResetFile();

            movieRatingApp.ImportData(true);

            Assert.AreEqual(20, movieRatingApp.DataCount);
        }

        [TestMethod]
        public void ImportData_NonExistentFile()
        {
            movieRatingApp.FilePath = "C:\\nonExistent.txt";
            if (File.Exists(movieRatingApp.FilePath))
            {
                File.Delete(movieRatingApp.FilePath);
            }

            Assert.ThrowsException<FileNotFoundException>(
                () => movieRatingApp.ImportData(true), 
                string.Format(Resources.ErrorMessage_FileNotFoundPattern, Path.GetFileName(movieRatingApp.FilePath), movieRatingApp.FilePath));
        }

        [TestMethod]
        public void ImportData_OneInvalidItem()
        {
            ResetFile();
            const string invalidItem = "a::b::c::d";
            File.AppendAllText(this.filePath, invalidItem);

            movieRatingApp.ImportData(true);

            Assert.AreEqual(20, movieRatingApp.DataCount);
        }

        #endregion

        #region GetTopTen tests
        
        [TestMethod]
        public void GetTopTen_PerfectFile()
        {
            ResetFile();
            movieRatingApp.ImportData(true);

            List<ReviewSummaryItem> topTen = movieRatingApp.GetTopTen();
            Assert.AreEqual(10, topTen.Count);

            for (int i = 1; i < topTen.Count; i++)
            {
                ReviewSummaryItem currentItem = topTen[i];
                ReviewSummaryItem itemOnTop = topTen[i - 1];
                Assert.IsTrue(currentItem.Rating <= itemOnTop.Rating);
            }
        }

        [TestMethod]
        public void GetTopTen_FileWithOnlyNineMovies()
        {
            ResetFile();
            string[] fileContent = File.ReadAllLines(this.filePath);
            fileContent[0] = string.Empty;
            fileContent[10] = string.Empty;
            File.Delete(this.filePath);
            File.WriteAllLines(this.filePath, fileContent);

            movieRatingApp.ImportData(true);

            List<ReviewSummaryItem> topTen = movieRatingApp.GetTopTen();
            Assert.AreEqual(9, topTen.Count);

            for (int i = 1; i < topTen.Count; i++)
            {
                ReviewSummaryItem currentItem = topTen[i];
                ReviewSummaryItem itemOnTop = topTen[i - 1];
                Assert.IsTrue(currentItem.Rating <= itemOnTop.Rating);
            }
        }

        #endregion

    }
}
