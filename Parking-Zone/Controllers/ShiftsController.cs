using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Parking_Zone.Data;
using Parking_Zone.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Parking_Zone.Controllers
{
    [Authorize]
    public class ShiftsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShiftsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var shifts = await _context.Shifts
                .Include(s => s.Operator)
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();
            return View(shifts);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Operators = new SelectList(
                await _context.Operators.Where(o => o.IsActive).ToListAsync(),
                "Id",
                "Name"
            );
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Shift shift)
        {
            if (ModelState.IsValid)
            {
                shift.CreatedAt = DateTime.UtcNow;
                _context.Add(shift);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Operators = new SelectList(
                await _context.Operators.Where(o => o.IsActive).ToListAsync(),
                "Id",
                "Name",
                shift.OperatorId
            );
            return View(shift);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shift = await _context.Shifts
                .Include(s => s.Operator)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (shift == null)
            {
                return NotFound();
            }

            ViewBag.Operators = new SelectList(
                await _context.Operators.Where(o => o.IsActive).ToListAsync(),
                "Id",
                "Name",
                shift.OperatorId
            );
            return View(shift);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Shift shift)
        {
            if (id != shift.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    shift.UpdatedAt = DateTime.UtcNow;
                    _context.Update(shift);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ShiftExists(shift.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.Operators = new SelectList(
                await _context.Operators.Where(o => o.IsActive).ToListAsync(),
                "Id",
                "Name",
                shift.OperatorId
            );
            return View(shift);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var shift = await _context.Shifts.FindAsync(id);
            if (shift != null)
            {
                _context.Shifts.Remove(shift);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ShiftExists(int id)
        {
            return await _context.Shifts.AnyAsync(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> EndShift(int id)
        {
            var shift = await _context.Shifts.FindAsync(id);
            if (shift != null && !shift.EndTime.HasValue)
            {
                shift.EndTime = DateTime.UtcNow;
                shift.UpdatedAt = DateTime.UtcNow;
                _context.Update(shift);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
