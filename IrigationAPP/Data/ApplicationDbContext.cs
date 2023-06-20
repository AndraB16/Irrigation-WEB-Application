using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using IrigationAPP.Models;

namespace IrigationAPP.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<IrigationAPP.Models.DataRead> DataRead { get; set; }
        public DbSet<IrigationAPP.Models.DataReadAccuWeather> DataReadAccuWeather { get; set; }
        public DbSet<IrigationAPP.Models.ValveState> ValveState { get; set; }
        public DbSet<IrigationAPP.Models.Schedule> Schedule { get; set; }
        public DbSet<IrigationAPP.Models.Users> Users { get; set; }
        public DbSet<IrigationAPP.Models.IrrigationTime> IrrigationTime { get; set; }
 
    }
}