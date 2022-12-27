using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using aspNETuniversity.Models;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace aspNETuniversity.Controllers
{
    public class StudentsController : Controller
    {
        private readonly univerContext _context;

        public StudentsController(univerContext context)
        {
            _context = context;
        }

        // GET: Students
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
                var students = _context.Students.Where(s => s.StudGroupCode.Contains(searchString) 
                || s.Name.Contains(searchString));
                students = students.Include(s => s.StudGroupCodeNavigation).Include(s => s.StudGroupCodeNavigation.SpecializationCodeNavigation)
                    .Include(s => s.StudGroupCodeNavigation.SpecializationCodeNavigation.FacultyCodeNavigation);
                return View(await students.ToPagedListAsync(pageNumber, pageSize));
            }

            else
            {
                var univerContext = _context.Students.Include(s => s.StudGroupCodeNavigation).Include(s => s.StudGroupCodeNavigation.SpecializationCodeNavigation)
                    .Include(s => s.StudGroupCodeNavigation.SpecializationCodeNavigation.FacultyCodeNavigation);
                return View(await univerContext.ToPagedListAsync(pageNumber, pageSize));
            }
        }

        // GET: Students/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.StudGroupCodeNavigation)
                .FirstOrDefaultAsync(m => m.Zachetka == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        [Authorize(Roles = "admin, dean")]
        public IActionResult Create()
        {
            string faculty = "";

            if (User.Claims.Any())
            {
                faculty = User.FindFirst(x => x.Type == "facultyID").Value;
            }

            ViewBag.Faculty = faculty;
            if (User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value == "dean")
            {
                ViewData["StudGroupCode"] = new SelectList(_context.StudGroups.Where(s => s.SpecializationCodeNavigation.FacultyCode== int.Parse(faculty)),
                    "StudGroupCode", "StudGroupCode");
            }
            else
            {
                ViewData["StudGroupCode"] = new SelectList(_context.StudGroups, "StudGroupCode", "StudGroupCode");
            }
            return View();

        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, dean")]
        public async Task<IActionResult> Create([Bind("Zachetka,Name,SalaryFather,SalaryMother,FamilyKol,StudGroupCode")] Student student)
        {
            TempData["Message"] = "Некорректно заполнены поля";

            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StudGroupCode"] = new SelectList(_context.StudGroups, "StudGroupCode", "StudGroupCode", student.StudGroupCode);
            return View(student);
        }

        // GET: Students/Edit/5
        [Authorize(Roles = "admin, dean")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
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
                ViewData["StudGroupCode"] = new SelectList(_context.StudGroups.Where(s => s.SpecializationCodeNavigation.FacultyCode == int.Parse(faculty)),
                    "StudGroupCode", "StudGroupCode", student.StudGroupCode);
            }
            else
            {
                ViewData["StudGroupCode"] = new SelectList(_context.StudGroups, "StudGroupCode", "StudGroupCode", student.StudGroupCode);
            }

            return View(student);

            

        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "admin, dean")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Zachetka,Name,SalaryFather,SalaryMother,FamilyKol,StudGroupCode")] Student student)
        {
            TempData["Message"] = "Некорректно заполнены поля";

            if (id != student.Zachetka)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Zachetka))
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
            ViewData["StudGroupCode"] = new SelectList(_context.StudGroups, "StudGroupCode", "StudGroupCode", student.StudGroupCode);
            return View(student);
        }

        // GET: Students/Delete/5
        [Authorize(Roles = "admin, dean")]
        public async Task<IActionResult> Delete(int? id)
        {
            var student = await _context.Students
                .Include(s => s.StudGroupCodeNavigation)
                .Include(s => s.StudGroupCodeNavigation.SpecializationCodeNavigation)
                .Include(s => s.StudGroupCodeNavigation.SpecializationCodeNavigation.FacultyCodeNavigation)
                .FirstOrDefaultAsync(m => m.Zachetka == id);

            if (student != null && User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value == "dean" &&
               !(User.FindFirst(x => x.Type == "facultyID").Value == student.StudGroupCodeNavigation.SpecializationCodeNavigation.FacultyCodeNavigation.FacultyCode.ToString()))
            {
                return RedirectToAction("Index", "Faculties");
            }
            else

            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [Authorize(Roles = "admin, dean")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Students == null)
            {
                return Problem("Entity set 'univerContext.Students'  is null.");
            }
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
          return _context.Students.Any(e => e.Zachetka == id);
        }
    }
}
