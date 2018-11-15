using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MovieRating.Models;

namespace MovieRating
{
    public class Aggregator
    {
        public List<UserReview> UserReviews { get; private set; }

        public Aggregator()
        {
            this.UserReviews = new List<UserReview>();
        }

        public bool Reset(bool force)
        {
            if (force || UserReviews.Count == 0)
            {
                this.UserReviews = new List<UserReview>();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Aggregate(List<UserReview> newCalculatedValues)
        {
            lock (new object())
            {
                this.UserReviews.AddRange(newCalculatedValues);
            }
        }

    }
}
