using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using MovieRating.Models;

namespace MovieRating
{
    public class MovieRatingApp
    {
        #region Fields

        private readonly byte threadsQuantity;
        private readonly Aggregator aggregator;
        private List<Thread> threads;
        private string[] fileContent;
        private List<UserReview> data;

        #endregion

        #region Properties

        public string FilePath { get; set; }

        public long DataCount => this.data.Count;

        #endregion

        #region Constructors

        public MovieRatingApp(string filePath)
        {
            this.threads = new List<Thread>();
            this.threadsQuantity = 4;
            this.aggregator = new Aggregator();
            this.data = new List<UserReview>();
            this.FilePath = filePath;
        }

        #endregion

        #region Public methods

        public void ReadFileContent()
        {
            if (string.IsNullOrEmpty(this.FilePath))
            {
                throw new InvalidDataException(Resources.ErrorMessage_FilePathEmpty);
            }
            else if (!File.Exists(this.FilePath))
            {
                throw new FileNotFoundException(string.Format(Resources.ErrorMessage_FileNotFoundPattern, Path.GetFileName(this.FilePath), this.FilePath));
            }
            else
            {
                this.fileContent = File.ReadAllLines(this.FilePath);
            }
        }

        public void ImportData(bool resetPreviousData = false)
        {
            if (resetPreviousData)
            {
                this.aggregator.Reset(true);
            }

            if (this.fileContent == null)
            {
                this.ReadFileContent();
            }

            int numLinesPerThread = this.fileContent.Length / this.threadsQuantity;
            for (int i = 0; i < this.threadsQuantity; i++)
            {
                string[] threadLines;
                if (i + 1 == this.threadsQuantity)
                {
                    threadLines = this.fileContent.Skip(numLinesPerThread * i).Take(this.fileContent.Length - numLinesPerThread * i).ToArray();
                }
                else
                {
                    threadLines = this.fileContent.Skip(numLinesPerThread * i).Take(numLinesPerThread).ToArray();
                }

                var threadCalculateLogic = new CalculatorThreadLogic(this.aggregator, threadLines);
                var thread = new Thread(threadCalculateLogic.Run);
                this.threads.Add(thread);
            }

            foreach (Thread thread in threads)
            {
                thread.Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            this.data.AddRange(this.aggregator.UserReviews);
            this.fileContent = null;
        }

        public List<ReviewSummaryItem> GetTopTen()
        {
            return this.data.GroupBy(userReview =>
                    userReview.MovieId,
                    (key, reviews) => new { key = key, rating = reviews.Average(review => review.Rating) })
                .OrderByDescending(review => review.rating)
                .Take(10)
                .Select(review => new ReviewSummaryItem() { MovieId = review.key, Rating = review.rating })
                .ToList();
        }

        public double GetTotalAverage()
        {
            return this.data.Average(userReview => userReview.Rating);
        }

        #endregion
    }
}
