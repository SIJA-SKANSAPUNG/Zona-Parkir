using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parking_Zone.Data;
using Parking_Zone.Models;
using System;
using System.Threading.Tasks;

namespace Parking_Zone.Controllers
{
    [Authorize]
    public class OperatorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OperatorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var operators = await _context.Operators
                .Include(o => o.Shifts)
                .ToListAsync();
            return View(operators);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Operator operatorModel)
        {
            if (ModelState.IsValid)
            {
                operatorModel.JoinDate = DateTime.UtcNow;
                operatorModel.IsActive = true;
                _context.Add(operatorModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(operatorModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var operatorModel = await _context.Operators
                .Include(o => o.Shifts)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (operatorModel == null)
            {
                return NotFound();
            }
            return View(operatorModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Operator operatorModel)
        {
            if (id != operatorModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(operatorModel);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await OperatorExists(operatorModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(operatorModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var operatorModel = await _context.Operators.FindAsync(id);
            if (operatorModel != null)
            {
                operatorModel.IsActive = false;
                _context.Update(operatorModel);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> OperatorExists(int id)
        {
            return await _context.Operators.AnyAsync(e => e.Id == id);
        }
    }
}
