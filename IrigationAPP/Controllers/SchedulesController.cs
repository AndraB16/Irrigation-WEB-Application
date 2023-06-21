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
    public class SchedulesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SchedulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Schedules
        public async Task<IActionResult> Index()
        {
            var schedules = await _context.Schedule.OrderByDescending(s => s.Id).Take(8).ToListAsync();
            var totalCount = await _context.Schedule.CountAsync();

            ViewData["TotalCount"] = totalCount;

            return View(schedules);
        }

        public async Task<IActionResult> Index2()
        {

            return View(await _context.Schedule.ToListAsync());

        }

        // GET: Schedules/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ValveId,StartTime,StopTime,Status")] Schedule schedule)
        {
            if (schedule.StartTime <= DateTime.Now)
            {
                ModelState.AddModelError("StartTime", "Start Time must be in the future.");
            }

            if (schedule.StopTime <= schedule.StartTime || schedule.StopTime > schedule.StartTime.AddHours(1))
            {
                ModelState.AddModelError("StopTime", "Stop Time must be no more than one hour later than Start Time.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(schedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(schedule);
        }

       

        // GET: Schedules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedule
                .FirstOrDefaultAsync(m => m.Id == id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // POST: Schedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schedule = await _context.Schedule.FindAsync(id);
            _context.Schedule.Remove(schedule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScheduleExists(int id)
        {
            return _context.Schedule.Any(e => e.Id == id);
        }
    }
}
