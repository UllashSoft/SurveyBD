using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SurveyBD.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public List<Worker> Workers { get; set; } = new();
        public List<RiverSurveyData> Data { get; set; } = new();
    }
}
