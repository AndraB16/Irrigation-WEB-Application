using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IrigationAPP.AccuWeatherServices
{
    public class CurrentCondition
    {
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

}
