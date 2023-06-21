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
            var dataReads = await _context.DataRead.OrderByDescending(x => x.time).Take(20).ToListAsync();
            var totalCount = await _context.DataRead.CountAsync();

            ViewData["TotalCount"] = totalCount;

            return View(dataReads);
        }


        public async Task<IActionResult> Index2()
        {
            return View(await _context.DataRead.ToListAsync());
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
