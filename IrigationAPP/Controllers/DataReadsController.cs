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
    public class DataReadsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DataReadsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DataReads
        public async Task<IActionResult> Index()
        {
            return View(await _context.DataRead.ToListAsync());
        }

        // GET: DataReads/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dataRead = await _context.DataRead
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dataRead == null)
            {
                return NotFound();
            }

            return View(dataRead);
        }

        // GET: DataReads/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DataReads/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,soilMoisturePercent,time,collectorId")] DataRead dataRead)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dataRead);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dataRead);
        }

        // GET: DataReads/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dataRead = await _context.DataRead.FindAsync(id);
            if (dataRead == null)
            {
                return NotFound();
            }
            return View(dataRead);
        }

        // POST: DataReads/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,soilMoisturePercent,time,collectorId")] DataRead dataRead)
        {
            if (id != dataRead.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dataRead);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DataReadExists(dataRead.Id))
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
            return View(dataRead);
        }

        // GET: DataReads/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dataRead = await _context.DataRead
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dataRead == null)
            {
                return NotFound();
            }

            return View(dataRead);
        }

        // POST: DataReads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dataRead = await _context.DataRead.FindAsync(id);
            _context.DataRead.Remove(dataRead);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DataReadExists(int id)
        {
            return _context.DataRead.Any(e => e.Id == id);
        }
    }
}
