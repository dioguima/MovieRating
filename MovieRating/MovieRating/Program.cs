using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using MovieRating.Models;

namespace MovieRating
{
    class Program
    {
        private const int THREAD_NUMBER = 4;

        static void Main(string[] args)
        {
            Console.WriteLine(Resources.Message_Initial);
            string filePath = Console.ReadLine();
            
            var movieRatingApp = new MovieRatingApp(filePath);
            
            movieRatingApp.ImportData(true);

            Console.WriteLine(Resources.Header_TopTen);
            List<ReviewSummaryItem> topTen = movieRatingApp.GetTopTen();
            for (int i = 0; i < topTen.Count; i++)
            {
                Console.WriteLine($"{i} = {topTen[i].MovieId} {topTen[i].Rating}");
            }

            Console.WriteLine(Resources.Header_TotalAverage);
            Console.WriteLine(movieRatingApp.GetTotalAverage().ToString("0.00"));
            
            Console.Read();
        }
        
    }
}
