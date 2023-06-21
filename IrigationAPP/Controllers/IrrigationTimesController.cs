using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IrigationAPP.Data;
using IrigationAPP.Models;

namespace IrigationAPP.Controllers
{
    public class IrrigationTimesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IrrigationTimesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: IrrigationTimes
        public async Task<IActionResult> Index()
        {
            var irrigationTime = await _context.IrrigationTime.OrderByDescending(s => s.time).Take(8).ToListAsync();
            var totalCount = await _context.IrrigationTime.CountAsync();

            ViewData["TotalCount"] = totalCount;

            return View(irrigationTime);
        }
        public async Task<IActionResult> Index2()
        {
            return View(await _context.IrrigationTime.ToListAsync());
        }

        public async Task<IActionResult> Index3()
        {
            return View(await _context.IrrigationTime.ToListAsync());
        }
    }
}
