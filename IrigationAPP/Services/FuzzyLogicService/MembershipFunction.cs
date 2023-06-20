using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IrigationAPP.Services.FuzzyLogicService
{
    public abstract class MembershipFunction
    {
        public abstract double GetMembershipDegree(double x);
    }
}
