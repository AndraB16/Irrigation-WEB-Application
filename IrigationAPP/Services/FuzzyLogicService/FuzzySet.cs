using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IrigationAPP.Services.FuzzyLogicService
{
    public class FuzzySet
    {
        public string Name { get; set; }
        public MembershipFunction MembershipFunction { get; set; }

        public FuzzySet(string name, MembershipFunction function)
        {
            Name = name;
            MembershipFunction = function;
        }

        public double GetMembershipDegree(double x)
        {
            return MembershipFunction.GetMembershipDegree(x);
        }
    }



}
