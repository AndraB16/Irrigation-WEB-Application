using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IrigationAPP.Services.FuzzyLogicService
{
    public class FuzzyRule
    {
        public List<(string, string)> Premise { get; set; }
        public (string, string) Conclusion { get; set; }

        public FuzzyRule(List<(string, string)> premise, (string, string) conclusion)
        {
            Premise = premise;
            Conclusion = conclusion;
        }
    }

}
