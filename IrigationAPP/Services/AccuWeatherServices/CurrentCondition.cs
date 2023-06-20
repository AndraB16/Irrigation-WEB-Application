using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IrigationAPP.AccuWeatherServices
{
    public class CurrentCondition
    {
        /*  public Temperature Temperature { get; set; }
          public PrecipitationSummary PrecipitationSummary { get; set; }*/
        public class TemperatureDetails
        {
            public class UnitDetails
            {
                public double Value { get; set; }
            }

            public UnitDetails Metric { get; set; }
        }

        public TemperatureDetails Temperature { get; set; }
        public bool HasPrecipitation { get; set; }
    }

    /*public class Temperature
    {
        public Imperial Imperial { get; set; }
    }

    public class Imperial
    {
        public double Value { get; set; }
        public string Unit { get; set; }
        public int UnitType { get; set; }
    }

    public class PrecipitationSummary
    {
        public Precipitation Precipitation { get; set; }
    }

    public class Precipitation
    {
        public int Probability { get; set; }
    }*/
}
