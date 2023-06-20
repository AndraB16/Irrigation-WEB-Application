using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IrigationAPP.Models
{
    public class ValveState
    {
        public int Id { get; set; }
        public int ValveId { get; set; }

        [Range(0, 1, ErrorMessage = "Invalid state value. Valid values are 0 (Off) and 1 (On).")]
        public int State { get; set; }

        public DateTime Time { get; set; }

        public ValveState()
        {

        }

    }
}
