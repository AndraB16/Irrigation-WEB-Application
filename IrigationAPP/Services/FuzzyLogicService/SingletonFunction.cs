using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IrigationAPP.Services.FuzzyLogicService
{
    public class SingletonFunction : MembershipFunction
    {
        private readonly double a;

        public SingletonFunction(double a)
        {
            this.a = a;
        }

        public override double GetMembershipDegree(double x)
        {
            return x == a ? 1 : 0;
        }
    }
}
