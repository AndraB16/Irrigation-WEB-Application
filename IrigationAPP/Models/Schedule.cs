﻿using System;
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
        public Schedule()
        {
                
        }

    }
}
