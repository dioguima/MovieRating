using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using MovieRating.Models;

namespace MovieRating
{
    public class CalculatorThreadLogic
    {
        private readonly Aggregator aggregator;
        private readonly string[] threadLines;

        public CalculatorThreadLogic(Aggregator aggregator, string[] threadLines)
        {
            this.aggregator = aggregator;
            this.threadLines = threadLines;
        }

        public void Run()
        {
            var userReviews = new List<UserReview>();

            foreach (string line in threadLines)
            {
                string[] data = line.Split("::");

                if (data.Length >= 4 &&
                    int.TryParse(data[0], out int userId) &&
                    int.TryParse(data[1], out int movieId) &&
                    byte.TryParse(data[2], out byte movieRating) &&
                    int.TryParse(data[3], out int timestamp))
                {
                    var newUserReview = new UserReview()
                    {
                        MovieId = movieId,
                        Rating = movieRating,
                        Timestamp = timestamp,
                        UserId = userId
                    };
                    userReviews.Add(newUserReview);
                }
                else
                {
                    Console.WriteLine($"Error: {line}");
                }
            }

            aggregator.Aggregate(userReviews);
        }
    }
}
