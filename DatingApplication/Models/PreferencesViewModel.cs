using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DatingApplication.Models
{
    public class PreferencesViewModel
    {
        public int? AgeFrom { get; set; }
        public int? AgeTo { get; set; }
        public int? WeightFrom { get; set; }
        public int? WeightTo { get; set; }
        public int? HeightFrom { get; set; }
        public int? HeightTo { get; set; }
        public List<hobbies> Hobbies { get; set; }
        public List<hobbies> SelectedHobbies { get; set; }
        public List<genders> Genders { get; set; }
        public List<genders> SelectedGenders { get; set; }
        public List<hair_colors> HairColors { get; set; }
        public List<hair_colors> SelectedHairColors { get; set; }
        public List<eye_colors> EyeColors { get; set; }
        public List<eye_colors> SelectedEyeColors { get; set; }
        public PostedPreferences PostedPreferences { get; set; }
    }

    public class PostedPreferences
    {
        public string[] HobbyIds { get; set; }
        public string[] Genders { get; set; }
        public string[] HairColors { get; set; }
        public string[] EyeColors { get; set; }
    }
}