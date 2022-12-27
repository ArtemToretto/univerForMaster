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
    public class StudGroupsController : Controller
    {
        private readonly univerContext _context;

        public StudGroupsController(univerContext context)
        {
            _context = context;
        }

        // GET: StudGroups
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
                var groups = _context.StudGroups.Where(s => s.StudGroupCode.Contains(searchString));
                groups = groups.Include(s => s.SpecializationCodeNavigation).Include(s => s.SpecializationCodeNavigation.FacultyCodeNavigation);
                return View(await groups.ToPagedListAsync(pageNumber, pageSize));
            }

            else
            {
                var univerContext = _context.StudGroups.Include(s => s.SpecializationCodeNavigation).Include(s => s.SpecializationCodeNavigation.FacultyCodeNavigation);
                return View(await univerContext.ToPagedListAsync(pageNumber, pageSize));
            }
        }

        // GET: StudGroups/Details/5
        [Authorize]
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
                ViewData["SpecializationCode"] = new SelectList(_context.Specializations.Where(s => s.FacultyCode==int.Parse(faculty)), "SpecCode", "SpecCode");
            }
            else
            {
                ViewData["SpecializationCode"] = new SelectList(_context.Specializations, "SpecCode", "SpecCode");
            }

            return View();
            
        }

        // POST: StudGroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, dean")]
        public async Task<IActionResult> Create([Bind("StudGroupCode,Year,SpecializationCode")] StudGroup studGroup)
        {
            TempData["Message"] = "Некорректно заполнены поля";

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
        [Authorize(Roles = "admin, dean")]
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

            string faculty = "";

            if (User.Claims.Any())
            {
                faculty = User.FindFirst(x => x.Type == "facultyID").Value;
            }

            ViewBag.Faculty = faculty;

            if (User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value == "dean")
            {
                ViewData["SpecializationCode"] = new SelectList(_context.Specializations.Where(s => s.FacultyCode == int.Parse(faculty)), "SpecCode", "SpecCode", studGroup.SpecializationCode);
            }
            else
            {
                ViewData["SpecializationCode"] = new SelectList(_context.Specializations, "SpecCode", "SpecCode", studGroup.SpecializationCode);
            }
            
            return View(studGroup);
        }

        // POST: StudGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, dean")]
        public async Task<IActionResult> Edit(string id, [Bind("StudGroupCode,Year,SpecializationCode")] StudGroup studGroup)
        {
            TempData["Message"] = "Некорректно заполнены поля";

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
        [Authorize(Roles = "admin, dean")]
        public async Task<IActionResult> Delete(string id)
        {
            var studGroup = await _context.StudGroups
                .Include(s => s.SpecializationCodeNavigation).Include(s => s.SpecializationCodeNavigation.FacultyCodeNavigation)
                .FirstOrDefaultAsync(m => m.StudGroupCode == id);

            if (studGroup != null && User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value == "dean" &&
               !(User.FindFirst(x => x.Type == "facultyID").Value == studGroup.SpecializationCodeNavigation.FacultyCodeNavigation.FacultyCode.ToString()))
            {
                return RedirectToAction("Index", "Faculties");
            }
            else
            {
                if (id == null || _context.StudGroups == null)
                {
                    return NotFound();
                }


                if (studGroup == null)
                {
                    return NotFound();
                }

                return View(studGroup);
            }  
        }

        // POST: StudGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "admin, dean")]
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
