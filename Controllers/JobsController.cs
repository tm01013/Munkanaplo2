using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Munkanaplo2.Data;
using Munkanaplo2.Models;

namespace Munkanaplo2.Controllers
{
    public class JobsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobsController(ApplicationDbContext context)
        {
            _context = context;
            ProjectsController.ShowIndex += OnShowIndexReceved;
        }

        // GET: Jobs
        public async Task<IActionResult> Index([Bind("Id")]int Id)
        {
            if(TempData.ContainsKey("ProjectId"))
            {
                ViewBag.ProjectId = TempData["ProjectId"];
                return View("Index", await _context.JobModel.Where(jm => jm.ProjectId == int.Parse(TempData["ProjectId"].ToString())).ToListAsync());
            }
            else
            {
            ViewBag.ProjectId = Id;
            return View("Index", await _context.JobModel.Where(jm => jm.ProjectId == Id).ToListAsync());
            }
        }

        public async Task<IActionResult> TeacherView([Bind("Id")]int Id)
        {
            ViewBag.ProjectId = Id;
            return View( await _context.JobModel.Where(jm => jm.ProjectId == Id).ToListAsync());
        }

        // GET: Jobs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.JobModel == null)
            {
                return NotFound();
            }

            var jobModel = await _context.JobModel
                .FirstOrDefaultAsync(m => m.Id == id);
#pragma warning disable 8602        
            ViewBag.ProjectId = jobModel.ProjectId;
#pragma warning restore 8602
            if (jobModel == null)
            {
                return NotFound();
            }

            return View(jobModel);
        }

        // GET: Jobs/Create
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id")]int Id)
        {
            var users = await _context.Users.ToListAsync();
            ViewBag.Users = new SelectList(users);
            ViewBag.ProjectId = Id;
            return View();
        }

        // POST: Jobs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost()]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateConfirmed([Bind("Id,JobTitle,JobDescription,JobOwner,JobCreator,CreationDate,JobStatus,FinishDate,ProjectId")] JobModel jobModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jobModel);
                await _context.SaveChangesAsync();

                ViewBag.ProjectId = jobModel.ProjectId;
                return View("Index", await _context.JobModel.Where(jm => jm.ProjectId == jobModel.ProjectId).ToListAsync());
            }
            return View(jobModel);
        }

        [Authorize]
        // GET: Jobs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.JobModel == null)
            {
                return NotFound();
            }

            var jobModel = await _context.JobModel.FindAsync(id);
            if (jobModel == null)
            {
                return NotFound();
            }
            var users = await _context.Users.ToListAsync();
            ViewBag.Users = new SelectList(users);
            ViewBag.ProjectId = jobModel.ProjectId;
            return View(jobModel);
        }

        // POST: Jobs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditConfirmed(int id, [Bind("Id,JobTitle,JobDescription,JobOwner,JobCreator,CreationDate,JobStatus,FinishDate,ProjectId")] JobModel jobModel)
        {
            if (id != jobModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jobModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobModelExists(jobModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["ProjectId"] = jobModel.ProjectId;
                return RedirectToAction("Index");
            }
            return View(jobModel);
        }

        [Authorize]
        // GET: Jobs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.JobModel == null)
            {
                return NotFound();
            }

            var jobModel = await _context.JobModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobModel == null)
            {
                return NotFound();
            }

            return View(jobModel);
        }

        // POST: Jobs/Delete/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([Bind("Id")]int id)
        {
            if (_context.JobModel == null)
            {
                return Problem("Entity set 'ApplicationDbContext.JobModel'  is null.");
            }
            var jobModel = await _context.JobModel.FindAsync(id);
            if (jobModel != null)
            {
                _context.JobModel.Remove(jobModel);
            }
            
            await _context.SaveChangesAsync();

            TempData["ProjectId"] = jobModel.ProjectId;
            return RedirectToAction("Index");
        }

        public void OnShowIndexReceved(int projectId, bool isShowTeacher)
        {
            if(!isShowTeacher)
            {
                Index(projectId);
            }
            else
            {
                TeacherView(projectId);
            }
        }

        private bool JobModelExists(int id)
        {
          return (_context.JobModel?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
