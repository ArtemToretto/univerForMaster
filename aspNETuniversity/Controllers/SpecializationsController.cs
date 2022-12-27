using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using aspNETuniversity.Models;
using X.PagedList;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace aspNETuniversity.Controllers
{
    public class SpecializationsController : Controller
    {
        private readonly univerContext _context;

        public SpecializationsController(univerContext context)
        {
            _context = context;
        }

        // GET: Specializations
        [Authorize]
        public async Task<IActionResult> Index(string searchString, int? page, string currentFilter)
        {
            string role = "";
            string faculty = "";

            if (User.Claims.Any())
            {
                role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
                faculty = User.FindFirst(x => x.Type == "facultyID").Value;
            }

            ViewBag.Role = role;
            ViewBag.Faculty = faculty;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var lol = ViewBag.SearchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                var spec = _context.Specializations.Where(s => s.Cvalification.Contains(searchString)
                                       || s.Name.Contains(searchString));
                spec = spec.Include(s => s.FacultyCodeNavigation);
                return View(await spec.ToPagedListAsync(pageNumber, pageSize));
            }
            
            else
            {
                var univerContext = _context.Specializations.Include(s => s.FacultyCodeNavigation);
                return View(await univerContext.ToPagedListAsync(pageNumber, pageSize));
            }
            

        }

        // GET: Specializations/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Specializations == null)
            {
                return NotFound();
            }

            var specialization = await _context.Specializations
                .Include(s => s.FacultyCodeNavigation)
                .FirstOrDefaultAsync(m => m.SpecCode == id);
            if (specialization == null)
            {
                return NotFound();
            }

            return View(specialization);
        }

        // GET: Specializations/Create
        [Authorize(Roles = "admin, dean")]
        public IActionResult Create()
        {
            string role = "";
            string faculty = "";

            if (User.Claims.Any())
            {
                role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
                faculty = User.FindFirst(x => x.Type == "facultyID").Value;
            }

            ViewBag.Role = role;
            ViewBag.Faculty = faculty;

            if (User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value == "dean")
            {
                ViewData["FacultyCode"] =User.FindFirst(x => x.Type == "facultyID").Value;
            }
            else
            {
                ViewData["FacultyCode"] = new SelectList(_context.Facultys, "FacultyCode", "FacultyCode");
            }
            return View();
        }

        // POST: Specializations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, dean")]
        public async Task<IActionResult> Create([Bind("SpecCode,Cvalification,Name,FacultyCode")] Specialization specialization)
        {
            TempData["Message"] = "Некорректно заполнены поля";

            if (ModelState.IsValid)
            {
                if (User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value == "dean")
                {
                    specialization.FacultyCode = int.Parse(User.FindFirst(x => x.Type == "facultyID").Value);
                }
                _context.Add(specialization);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            if (User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value == "dean")
            {
                ViewData["FacultyCode"] = User.FindFirst(x => x.Type == "facultyID").Value;
            }
            else
            {
                ViewData["FacultyCode"] = new SelectList(_context.Facultys, "FacultyCode", "FacultyCode");
            }
            return View(specialization);
        }

        // GET: Specializations/Edit/5
        [Authorize(Roles = "admin, dean")]
        public async Task<IActionResult> Edit(int? id)
        {

            //var specialization = await _context.Specializations.FindAsync(id);
            var specialization = await _context.Specializations.Include(s => s.FacultyCodeNavigation).FirstOrDefaultAsync(m => m.SpecCode == id);
            if (specialization != null && User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value == "dean" &&
               !(User.FindFirst(x => x.Type == "facultyID").Value == specialization.FacultyCodeNavigation.FacultyCode.ToString()))
            {
                return RedirectToAction("Index", "Faculties");
            }
            else
            {
                if (id == null || _context.Specializations == null)
                {
                    return NotFound();
                }

                if (specialization == null)
                {
                    return NotFound();
                }
                ViewData["FacultyCode"] = new SelectList(_context.Facultys, "FacultyCode", "FacultyCode", specialization.FacultyCode);
                return View(specialization);
            }
        }

        // POST: Specializations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, dean")]
        public async Task<IActionResult> Edit(int id, [Bind("SpecCode,Cvalification,Name,FacultyCode")] Specialization specialization)
        {
            TempData["Message"] = "Некорректно заполнены поля";

            if (id != specialization.SpecCode)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(specialization);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!SpecializationExists(specialization.SpecCode))
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
                ViewData["FacultyCode"] = new SelectList(_context.Facultys, "FacultyCode", "FacultyCode", specialization.FacultyCode);
                return View(specialization);
        }

        // GET: Specializations/Delete/5
        [Authorize(Roles = "admin, dean")]
        public async Task<IActionResult> Delete(int? id)
        {
            var specialization = await _context.Specializations.Include(s => s.FacultyCodeNavigation).FirstOrDefaultAsync(m => m.SpecCode == id);
            if (specialization != null && User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value == "dean" &&
               !(User.FindFirst(x => x.Type == "facultyID").Value == specialization.FacultyCodeNavigation.FacultyCode.ToString()))
            {
                return RedirectToAction("Index", "Faculties");
            }
            else
            {
                if (id == null || _context.Specializations == null)
                {
                    return NotFound();
                }

                if (specialization == null)
                {
                    return NotFound();
                }

                return View(specialization);
            }

        }

        // POST: Specializations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, dean")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var specialization = await _context.Specializations.FindAsync(id);
                if (_context.Specializations == null)
                {
                    return Problem("Entity set 'univerContext.Specializations'  is null.");
                }

                if (specialization != null)
                {
                    _context.Specializations.Remove(specialization);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); 
        }

        private bool SpecializationExists(int id)
        {
          return _context.Specializations.Any(e => e.SpecCode == id);
        }
    }
}
