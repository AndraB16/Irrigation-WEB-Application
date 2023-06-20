using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IrigationAPP.Models
{
    public class Schedule
    {
        public int Id { get; set; }
        public int ValveId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        public String Status { get; set; }
        /*public string Status
        {
            get
            {
                if (DateTime.Now < StartTime)
                    return "Waiting";
                else if (DateTime.Now >= StartTime && DateTime.Now <= StopTime)
                    return "In Progress";
                else
                    return "Finished";
            }*
        }*/
        public Schedule()
        {
                
        }

    }
}
