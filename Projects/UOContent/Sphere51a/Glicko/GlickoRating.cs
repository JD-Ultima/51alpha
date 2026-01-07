using System;

namespace Server.Sphere51a.Glicko
{
    /// <summary>
    /// Represents a Glicko-2 rating with all three parameters.
    /// </summary>
    public class GlickoRating
    {
        /// <summary>
        /// Player's rating (mu) - estimate of player's skill
        /// </summary>
        public decimal Rating { get; set; }

        /// <summary>
        /// Rating deviation (phi) - uncertainty in the rating estimate
        /// </summary>
        public decimal RatingDeviation { get; set; }

        /// <summary>
        /// Volatility (sigma) - degree of expected fluctuation in player's rating
        /// </summary>
        public decimal Volatility { get; set; }

        /// <summary>
        /// Last updated timestamp
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Total number of matches played
        /// </summary>
        public int TotalMatches { get; set; }

        /// <summary>
        /// Number of wins
        /// </summary>
        public int Wins { get; set; }

        /// <summary>
        /// Number of losses
        /// </summary>
        public int Losses { get; set; }

        /// <summary>
        /// Create a new Glicko rating with default values for new players
        /// </summary>
        public GlickoRating()
        {
            Rating = 1500.0m;
            RatingDeviation = 350.0m;
            Volatility = 0.06m;
            LastUpdated = DateTime.UtcNow;
            TotalMatches = 0;
            Wins = 0;
            Losses = 0;
        }

        /// <summary>
        /// Create a Glicko rating with specific values
        /// </summary>
        public GlickoRating(decimal rating, decimal ratingDeviation, decimal volatility)
        {
            Rating = rating;
            RatingDeviation = ratingDeviation;
            Volatility = volatility;
            LastUpdated = DateTime.UtcNow;
        }

        /// <summary>
        /// Create a complete Glicko rating with match statistics
        /// </summary>
        public GlickoRating(decimal rating, decimal ratingDeviation, decimal volatility,
                          int totalMatches, int wins, int losses)
        {
            Rating = rating;
            RatingDeviation = ratingDeviation;
            Volatility = volatility;
            TotalMatches = totalMatches;
            Wins = wins;
            Losses = losses;
            LastUpdated = DateTime.UtcNow;
        }

        /// <summary>
        /// Clone this rating
        /// </summary>
        public GlickoRating Clone()
        {
            return new GlickoRating
            {
                Rating = Rating,
                RatingDeviation = RatingDeviation,
                Volatility = Volatility,
                LastUpdated = LastUpdated,
                TotalMatches = TotalMatches,
                Wins = Wins,
                Losses = Losses
            };
        }
    }
}
