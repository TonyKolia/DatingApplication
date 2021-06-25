using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatingApplication.Models
{
    public class AdminStatsViewModel
    {
        public decimal PremiumUsersPerc { get; set; }
        public decimal SimpleUsersPerc { get; set; }
        public int NoOfNewUsers { get; set; }

        public decimal WomenPerc { get; set; }
        public decimal MenPerc { get; set; }
        public decimal AverageAge { get; set; }

        public List<string> PopularHobbies { get; set; }

        public int VisitsLastWeek { get; set; }
        public int RatingsLastWeek { get; set; }

    }
}