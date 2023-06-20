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
            return View(await _context.IrrigationTime.ToListAsync());
        }

        // GET: IrrigationTimes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var irrigationTime = await _context.IrrigationTime
                .FirstOrDefaultAsync(m => m.Id == id);
            if (irrigationTime == null)
            {
                return NotFound();
            }

            return View(irrigationTime);
        }

        // GET: IrrigationTimes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: IrrigationTimes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,irrigationTime,time")] IrrigationTime irrigationTime)
        {
            if (ModelState.IsValid)
            {
                _context.Add(irrigationTime);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(irrigationTime);
        }

        // GET: IrrigationTimes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var irrigationTime = await _context.IrrigationTime.FindAsync(id);
            if (irrigationTime == null)
            {
                return NotFound();
            }
            return View(irrigationTime);
        }

        // POST: IrrigationTimes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,irrigationTime,time")] IrrigationTime irrigationTime)
        {
            if (id != irrigationTime.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(irrigationTime);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IrrigationTimeExists(irrigationTime.Id))
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
            return View(irrigationTime);
        }

        // GET: IrrigationTimes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var irrigationTime = await _context.IrrigationTime
                .FirstOrDefaultAsync(m => m.Id == id);
            if (irrigationTime == null)
            {
                return NotFound();
            }

            return View(irrigationTime);
        }

        // POST: IrrigationTimes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var irrigationTime = await _context.IrrigationTime.FindAsync(id);
            _context.IrrigationTime.Remove(irrigationTime);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IrrigationTimeExists(int id)
        {
            return _context.IrrigationTime.Any(e => e.Id == id);
        }
    }
}
