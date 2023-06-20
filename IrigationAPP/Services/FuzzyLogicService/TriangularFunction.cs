using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IrigationAPP.Services.FuzzyLogicService
{
    public class TriangularFunction : MembershipFunction
    {
        private readonly double a;
        private readonly double b;

        public TriangularFunction(double a, double b)
        {
            this.a = a;
            this.b = b;
        }

        public override double GetMembershipDegree(double x)
        {
            if (x <= a) return 0;
            if (x > a && x <= b) return 1 - (b - x) / (b - a);
            if (x > b) return 0;

            throw new InvalidOperationException("Invalid input.");
        }
    }
}
