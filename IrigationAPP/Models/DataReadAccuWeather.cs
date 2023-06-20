using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IrigationAPP.Models
{
    public class DataReadAccuWeather
    {
        public int Id { get; set; }

        public int temperature { get; set; }
        public int rainProbability { get; set; }
        public DateTime time { get; set; }

        public DataReadAccuWeather()
        {

        }
    }
}
