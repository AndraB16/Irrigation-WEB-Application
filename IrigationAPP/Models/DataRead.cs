using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IrigationAPP.Models
{
    public class DataRead
    {
        public int Id { get; set; }

        public int collectorId { get; set; }
        public int soilMoisturePercent { get; set; }
        public DateTime time { get; set; }

        public DataRead()
        {

        }
    }
}
