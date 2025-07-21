using System;

namespace SurveyBD.Models
{
    public class RiverSurveyData
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double RiverBedLevel { get; set; }
        public double RiverBankLevel { get; set; }
        public double WaterLevel { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
