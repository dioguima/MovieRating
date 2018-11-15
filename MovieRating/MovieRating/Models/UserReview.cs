using System;
using System.Collections.Generic;
using System.Text;

namespace MovieRating.Models
{
    public class UserReview
    {
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public byte Rating { get; set; }
        public int Timestamp { get; set; }
    }
}
