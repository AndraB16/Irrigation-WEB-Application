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

        // GET: DataReadAccuWeathers
        public async Task<IActionResult> Index()
        {
            return View(await _context.DataReadAccuWeather.ToListAsync());
        }

        // GET: DataReadAccuWeathers/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: DataReadAccuWeathers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DataReadAccuWeathers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,temperature,rainProbability,time")] DataReadAccuWeather dataReadAccuWeather)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dataReadAccuWeather);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dataReadAccuWeather);
        }

        // GET: DataReadAccuWeathers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dataReadAccuWeather = await _context.DataReadAccuWeather.FindAsync(id);
            if (dataReadAccuWeather == null)
            {
                return NotFound();
            }
            return View(dataReadAccuWeather);
        }

        // POST: DataReadAccuWeathers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,temperature,rainProbability,time")] DataReadAccuWeather dataReadAccuWeather)
        {
            if (id != dataReadAccuWeather.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dataReadAccuWeather);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DataReadAccuWeatherExists(dataReadAccuWeather.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dataReadAccuWeather);
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
