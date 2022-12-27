using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using aspNETuniversity.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace aspNETuniversity.Controllers
{
    public class FacultiesController : Controller
    {
        private readonly univerContext _context;

        public FacultiesController(univerContext context)
        {
            _context = context;
        }

        [Authorize]
        // GET: Faculties
        public async Task<IActionResult> Index(string searchString)
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

            if (!String.IsNullOrEmpty(searchString))
            {
                var facultys = _context.Facultys.Where(s => s.FacultyName.Contains(searchString)
                                       || s.DeanName.Contains(searchString));
                return View(await facultys.ToListAsync());
            }
            else
            {
                return View(await _context.Facultys.ToListAsync());
            }
        }

        [Authorize]
        // GET: Faculties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Facultys == null)
            {
                return NotFound();
            }

            var faculty = await _context.Facultys
                .FirstOrDefaultAsync(m => m.FacultyCode == id);
            if (faculty == null)
            {
                return NotFound();
            }

            return View(faculty);
        }

        // GET: Faculties/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Faculties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FacultyCode,DeanName,FacultyName")] Faculty faculty)
        {
            TempData["Message"] = "Некорректно заполнены поля";
            if (ModelState.IsValid)
            {
                _context.Add(faculty);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(faculty);
        }

        // GET: Faculties/Edit/5
        [Authorize(Roles = "admin, dean")]
        public async Task<IActionResult> Edit(int? id)
        {
            //return RedirectToAction("Index", "Faculties");
            if (User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value=="dean" &&
               !(User.FindFirst(x => x.Type == "facultyID").Value==id.ToString()))
            {
                return RedirectToAction("Index", "Faculties");
            }
            else
            {
                if (id == null || _context.Facultys == null)
                {
                    return NotFound();
                }

                var faculty = await _context.Facultys.FindAsync(id);
                if (faculty == null)
                {
                    return NotFound();
                }
                return View(faculty);
            }
        }

        // POST: Faculties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [Authorize(Roles = "admin, dean")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FacultyCode,DeanName,FacultyName")] Faculty faculty)
        {
            TempData["Message"] = "Некорректно заполнены поля";
            if (User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value == "dean" &&
               !(User.FindFirst(x => x.Type == "facultyID").Value == id.ToString()))
            {
                return RedirectToAction("Index", "Faculties");
            }
            else
            {
                if (id != faculty.FacultyCode)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(faculty);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!FacultyExists(faculty.FacultyCode))
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
                return View(faculty);
            } 
        }

        // GET: Faculties/Delete/5
        [Authorize(Roles = "admin, dean")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value == "dean" &&
               !(User.FindFirst(x => x.Type == "facultyID").Value == id.ToString()))
            {
                return RedirectToAction("Index", "Faculties");
            }
            else
            {
                if (id == null || _context.Facultys == null)
                {
                    return NotFound();
                }

                var faculty = await _context.Facultys
                    .FirstOrDefaultAsync(m => m.FacultyCode == id);
                if (faculty == null)
                {
                    return NotFound();
                }

                return View(faculty);
            }
            
        }

        // POST: Faculties/Delete/5
        [Authorize(Roles = "admin, dean")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value == "dean" &&
               !(User.FindFirst(x => x.Type == "facultyID").Value == id.ToString()))
            {
                return RedirectToAction("Index", "Faculties");
            }
            else
            {
                if (_context.Facultys == null)
                {
                    return Problem("Entity set 'univerContext.Facultys'  is null.");
                }
                var faculty = await _context.Facultys.FindAsync(id);
                if (faculty != null)
                {
                    _context.Facultys.Remove(faculty);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }

        private bool FacultyExists(int id)
        {
          return _context.Facultys.Any(e => e.FacultyCode == id);
        }
    }
}
