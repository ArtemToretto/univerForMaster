using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using aspNETuniversity.Models;

namespace aspNETuniversity.Controllers
{
    public class StudGroupsController : Controller
    {
        private readonly univerContext _context;

        public StudGroupsController(univerContext context)
        {
            _context = context;
        }

        // GET: StudGroups
        public async Task<IActionResult> Index()
        {
            var univerContext = _context.StudGroups.Include(s => s.SpecializationCodeNavigation);
            return View(await univerContext.ToListAsync());
        }

        // GET: StudGroups/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.StudGroups == null)
            {
                return NotFound();
            }

            var studGroup = await _context.StudGroups
                .Include(s => s.SpecializationCodeNavigation)
                .FirstOrDefaultAsync(m => m.StudGroupCode == id);
            if (studGroup == null)
            {
                return NotFound();
            }

            return View(studGroup);
        }

        // GET: StudGroups/Create
        public IActionResult Create()
        {
            ViewData["SpecializationCode"] = new SelectList(_context.Specializations, "SpecCode", "SpecCode");
            return View();
        }

        // POST: StudGroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudGroupCode,Year,SpecializationCode")] StudGroup studGroup)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studGroup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SpecializationCode"] = new SelectList(_context.Specializations, "SpecCode", "SpecCode", studGroup.SpecializationCode);
            return View(studGroup);
        }

        // GET: StudGroups/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.StudGroups == null)
            {
                return NotFound();
            }

            var studGroup = await _context.StudGroups.FindAsync(id);
            if (studGroup == null)
            {
                return NotFound();
            }
            ViewData["SpecializationCode"] = new SelectList(_context.Specializations, "SpecCode", "SpecCode", studGroup.SpecializationCode);
            return View(studGroup);
        }

        // POST: StudGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("StudGroupCode,Year,SpecializationCode")] StudGroup studGroup)
        {
            if (id != studGroup.StudGroupCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studGroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudGroupExists(studGroup.StudGroupCode))
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
            ViewData["SpecializationCode"] = new SelectList(_context.Specializations, "SpecCode", "SpecCode", studGroup.SpecializationCode);
            return View(studGroup);
        }

        // GET: StudGroups/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.StudGroups == null)
            {
                return NotFound();
            }

            var studGroup = await _context.StudGroups
                .Include(s => s.SpecializationCodeNavigation)
                .FirstOrDefaultAsync(m => m.StudGroupCode == id);
            if (studGroup == null)
            {
                return NotFound();
            }

            return View(studGroup);
        }

        // POST: StudGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.StudGroups == null)
            {
                return Problem("Entity set 'univerContext.StudGroups'  is null.");
            }
            var studGroup = await _context.StudGroups.FindAsync(id);
            if (studGroup != null)
            {
                _context.StudGroups.Remove(studGroup);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudGroupExists(string id)
        {
          return _context.StudGroups.Any(e => e.StudGroupCode == id);
        }
    }
}
