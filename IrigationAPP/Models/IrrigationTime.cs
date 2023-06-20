using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IrigationAPP.Models
{
    public class IrrigationTime
    {
        public int Id { get; set; }
        public double irrigationTime { get; set; }
        public DateTime time{ get; set; }

        public IrrigationTime()
        {

        }
    }
}
