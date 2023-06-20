using IrigationAPP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using IrigationAPP.Models;

namespace IrigationAPP.Services.FuzzyLogicService
{

    public class FuzzyLogicServices 
    {

        private readonly ApplicationDbContext _dbContext;

        public FuzzyLogicServices(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private readonly List<FuzzySet> soil_moisture = new List<FuzzySet>
        {
            new FuzzySet("Dry", new TrapezoidalFunction(0, 40)),
            new FuzzySet("Moderate", new TriangularFunction(20, 80)),
            new FuzzySet("Wet", new TrapezoidalFunction(60, 100))
        };

        private readonly List<FuzzySet> rain_probability = new List<FuzzySet>
        {
            new FuzzySet("No", new SingletonFunction(0)),
            new FuzzySet("Yes", new SingletonFunction(1))
        };

        private readonly List<FuzzySet> air_temperature = new List<FuzzySet>
        {
            new FuzzySet("Cold", new TrapezoidalFunction(-30, 5)),
            new FuzzySet("Moderate", new TriangularFunction(0, 20)),
            new FuzzySet("Hot", new TrapezoidalFunction(15, 50))
        };

        private readonly List<FuzzySet> irigation_time = new List<FuzzySet>
        {
            new FuzzySet("None", new TriangularFunction(0, 0)),
            new FuzzySet("Short", new TriangularFunction(0, 4)),
            new FuzzySet("Medium", new TriangularFunction(2, 6)),
            new FuzzySet("Long", new TriangularFunction(4, 8)),
            new FuzzySet("Very_Long", new TrapezoidalFunction(6, 10))
        };

        public List<FuzzySet> GetFuzzySets(string variableName)
        {
            switch (variableName)
            {
                case "soil_moisture":
                    return soil_moisture;
                case "rain_probability":
                    return rain_probability;
                case "air_temperature":
                    return air_temperature;
                case "irigation_time":
                    return irigation_time;
                default:
                    throw new ArgumentException($"Unknown variable: {variableName}");
            }
        }
        public async Task<WeatherData> TakeDataAsync()
        {
            var moistureReadings = await _dbContext.DataRead
                .OrderByDescending(d => d.time)
                .Take(4)
                .Select(d => d.soilMoisturePercent)
                .ToListAsync();

            var averageMoisture = moistureReadings.Average();

            var lastWeatherData = await _dbContext.DataReadAccuWeather
                .OrderByDescending(d => d.time)
                .FirstAsync();

            double temperature = lastWeatherData.temperature;
            double rainProbability = lastWeatherData.rainProbability;

            return new WeatherData
            {
                AverageMoisture = averageMoisture,
                Temperature = temperature,
                RainProbability = rainProbability
            };
        }
        public Dictionary<string, double> Fuzzify(string variableName, double value)
        {
            var variableSets = GetFuzzySets(variableName);
            var result = new Dictionary<string, double>();

            foreach (var set in variableSets)
            {
                result[set.Name] = set.GetMembershipDegree(value);
            }

            return result;
        }
        public async Task PerformFuzzyfication()
        {
            var data = await TakeDataAsync();

            var temperatureMembershipDegrees = Fuzzify("air_temperature", data.Temperature);
            var soilMoistureMembershipDegrees = Fuzzify("soil_moisture", data.AverageMoisture);
            var rainProbabilityMembershipDegrees = Fuzzify("rain_probability", data.RainProbability);

            Dictionary<string, Dictionary<string, double>> fuzzifiedValues = new Dictionary<string, Dictionary<string, double>>
            {
                { "air_temperature", temperatureMembershipDegrees },
                { "soil_moisture", soilMoistureMembershipDegrees },
                { "rain_probability", rainProbabilityMembershipDegrees },
            };

            ApplyRules(fuzzifiedValues);
        }

        private List<FuzzyRule> rules = new List<FuzzyRule>
        {
            new FuzzyRule(
                new List<(string, string)> {
                    ("soil_moisture", "Dry"),
                    ("rain_probability", "No"),
                    ("air_temperature", "Cold")
                },
                ("irrigation_time", "Long")),
            new FuzzyRule(
                new List<(string, string)> {
                    ("soil_moisture", "Dry"),
                    ("rain_probability", "No"),
                    ("air_temperature", "Moderate")
                },
                ("irrigation_time", "Very_Long")),
            new FuzzyRule(
                new List<(string, string)> {
                    ("soil_moisture", "Dry"),
                    ("rain_probability", "No"),
                    ("air_temperature", "Hot")
                },
                ("irrigation_time", "Very_Long")),
            new FuzzyRule(
                new List<(string, string)> {
                    ("soil_moisture", "Dry"),
                    ("rain_probability", "Yes"),
                    ("air_temperature", "Cold")
                },
                ("irrigation_time", "Medium")),
            new FuzzyRule(
                new List<(string, string)> {
                    ("soil_moisture", "Dry"),
                    ("rain_probability", "Yes"),
                    ("air_temperature", "Moderate")
                },
                ("irrigation_time", "Long")),
            new FuzzyRule(
                new List<(string, string)> {
                    ("soil_moisture", "Dry"),
                    ("rain_probability", "Yes"),
                    ("air_temperature", "Hot")
                },
                ("irrigation_time", "Long")),
            new FuzzyRule(
                new List<(string, string)> {
                    ("soil_moisture", "Moderate"),
                    ("rain_probability", "No"),
                    ("air_temperature", "Cold")
                },
                ("irrigation_time", "Short")),
            new FuzzyRule(
                new List<(string, string)> {
                    ("soil_moisture", "Moderate"),
                    ("rain_probability", "No"),
                    ("air_temperature", "Moderate")
                },
                ("irrigation_time", "Medium")),
            new FuzzyRule(
                new List<(string, string)> {
                    ("soil_moisture", "Moderate"),
                    ("rain_probability", "No"),
                    ("air_temperature", "Hot")
                },
                ("irrigation_time", "Long")),
            new FuzzyRule(
                new List<(string, string)> {
                    ("soil_moisture", "Moderate"),
                    ("rain_probability", "Yes"),
                    ("air_temperature", "Cold")
                },
                ("irrigation_time", "None")),
            new FuzzyRule(
                new List<(string, string)> {
                    ("soil_moisture", "Moderate"),
                    ("rain_probability", "Yes"),
                    ("air_temperature", "Moderate")
                },
                ("irrigation_time", "Short")),
            new FuzzyRule(
                new List<(string, string)> {
                    ("soil_moisture", "Moderate"),
                    ("rain_probability", "Yes"),
                    ("air_temperature", "Hot")
                },
                ("irrigation_time", "Medium")),
            new FuzzyRule(
                new List<(string, string)> {
                    ("soil_moisture", "Wet"),
                    ("rain_probability", "No"),
                    ("air_temperature", "Cold")
                },
                ("irrigation_time", "None")),
            new FuzzyRule(
                new List<(string, string)> {
                    ("soil_moisture", "Wet"),
                    ("rain_probability", "No"),
                    ("air_temperature", "Moderate")
                },
                ("irrigation_time", "None")),
            new FuzzyRule(
                new List<(string, string)> {
                    ("soil_moisture", "Wet"),
                    ("rain_probability", "No"),
                    ("air_temperature", "Hot")
                },
                ("irrigation_time", "None")),
            new FuzzyRule(
                new List<(string, string)> {
                    ("soil_moisture", "Wet"),
                    ("rain_probability", "Yes"),
                    ("air_temperature", "Cold")
                },
                ("irrigation_time", "None")),
            new FuzzyRule(
                new List<(string, string)> {
                    ("soil_moisture", "Wet"),
                    ("rain_probability", "Yes"),
                    ("air_temperature", "Moderate")
                },
                ("irrigation_time", "None")),
            new FuzzyRule(
                new List<(string, string)> {
                    ("soil_moisture", "Wet"),
                    ("rain_probability", "Yes"),
                    ("air_temperature", "Hot")
                },
                ("irrigation_time", "None")),
        };

        public Dictionary<string, double> ApplyRules(Dictionary<string, Dictionary<string, double>> fuzzifiedValues)
        {
            Dictionary<string, double> outputValues = new Dictionary<string, double>();

            foreach (var rule in rules)
            {
                double minValue = double.MaxValue;

                foreach (var condition in rule.Premise)
                {
                    double value = fuzzifiedValues[condition.Item1][condition.Item2];
                    if (value < minValue)
                    {
                        minValue = value;
                    }
                }

                if (!outputValues.ContainsKey(rule.Conclusion.Item2) || minValue > outputValues[rule.Conclusion.Item2])
                {
                    outputValues[rule.Conclusion.Item2] = minValue;
                }
            }

            return outputValues;
        }


        public double Defuzzify(Dictionary<string, double> outputValues)
        {
            double sum = 0;
            double weightSum = 0;

            for (double x = 0; x <= 10; x += 0.1)
            {
                double highestMembership = 0;
                foreach (var entry in outputValues)
                {
                    var set = GetFuzzySets("irigation_time").First(s => s.Name == entry.Key);
                    double membership = Math.Min(entry.Value, set.GetMembershipDegree(x));
                    if (membership > highestMembership)
                    {
                        highestMembership = membership;
                    }
                }

                sum += x * highestMembership;
                weightSum += highestMembership;
            }

            if (weightSum != 0)
            {
                return sum / weightSum;
            }
            else
            {
                throw new Exception("No rules were triggered, defuzzification is not possible.");
            }
        }

        

     /*   public double Defuzzify(Dictionary<string, double> outputValues)
        {
            List<double> maximaValues = new List<double>();
            double highestMembership = 0;

            for (double x = 0; x <= 10; x += 0.1)
            {
                foreach (var entry in outputValues)
                {
                    var set = GetFuzzySets("irigation_time").First(s => s.Name == entry.Key);
                    double membership = Math.Min(entry.Value, set.GetMembershipDegree(x));

                    if (membership > highestMembership)
                    {
                        highestMembership = membership;
                        maximaValues.Clear();
                        maximaValues.Add(x);
                    }
                    else if (membership == highestMembership)
                    {
                        maximaValues.Add(x);
                    }
                }
            }

            if (maximaValues.Count > 0)
            {
                // Calculate the average of maxima
                return maximaValues.Average();
            }
            else
            {
                throw new Exception("No rules were triggered, defuzzification is not possible.");
            }
        }*/

        public async Task<double> DetermineIrigationTime()
        {
            var data = await TakeDataAsync();

            var temperatureMembershipDegrees = Fuzzify("air_temperature", data.Temperature);
            var soilMoistureMembershipDegrees = Fuzzify("soil_moisture", data.AverageMoisture);
            var rainProbabilityMembershipDegrees = Fuzzify("rain_probability", data.RainProbability);

            Dictionary<string, Dictionary<string, double>> fuzzifiedValues = new Dictionary<string, Dictionary<string, double>>
            {
                { "air_temperature", temperatureMembershipDegrees },
                { "soil_moisture", soilMoistureMembershipDegrees },
                { "rain_probability", rainProbabilityMembershipDegrees },
            };

            var outputValues = ApplyRules(fuzzifiedValues);

            var irigationTime = Defuzzify(outputValues);

            return irigationTime;
            
        }


    }

    public class WeatherData
    {
        public double AverageMoisture { get; set; }
        public double Temperature { get; set; }
        public double RainProbability { get; set; }
    }

}
