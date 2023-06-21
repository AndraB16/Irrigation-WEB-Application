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
    public class DataReadAccuWeathersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DataReadAccuWeathersController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var dataReadsAccu = await _context.DataReadAccuWeather.OrderByDescending(x => x.time).Take(20).ToListAsync();
            var totalCount = await _context.DataReadAccuWeather.CountAsync();

            ViewData["TotalCount"] = totalCount;

            return View(dataReadsAccu);
        }

        public async Task<IActionResult> Index2()
        {
            return View(await _context.DataReadAccuWeather.ToListAsync());
        }

        // GET: DataReadAccuWeathers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dataReadAccuWeather = await _context.DataReadAccuWeather
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dataReadAccuWeather == null)
            {
                return NotFound();
            }

            return View(dataReadAccuWeather);
        }

        // POST: DataReadAccuWeathers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dataReadAccuWeather = await _context.DataReadAccuWeather.FindAsync(id);
            _context.DataReadAccuWeather.Remove(dataReadAccuWeather);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DataReadAccuWeatherExists(int id)
        {
            return _context.DataReadAccuWeather.Any(e => e.Id == id);
        }
    }
}
