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
        }

        // GET: Jobs
        public async Task<IActionResult> Index()
        {
            return _context.JobModel != null ?
                        View(await _context.JobModel.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.JobModel'  is null.");
        }

        public async Task<IActionResult> TeacherView()
        {
            return _context.JobModel != null ?
                        View(await _context.JobModel.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.JobModel'  is null.");
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
            if (jobModel == null)
            {
                return NotFound();
            }

            return View(jobModel);
        }

        // GET: Jobs/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var users = await _context.Users.ToListAsync();
            ViewBag.Users = new SelectList(users);
            return View();
        }

        // POST: Jobs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost()]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,JobTitle,JobDescription,JobOwner,JobCreator,CreationDate,JobStatus,FinishDate")] JobModel jobModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jobModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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
            return View(jobModel);
        }

        // POST: Jobs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditConfirmed(int id, [Bind("Id,JobTitle,JobDescription,JobOwner,JobCreator,CreationDate,JobStatus,FinishDate")] JobModel jobModel)
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
                return RedirectToAction(nameof(Index));
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
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
            return RedirectToAction(nameof(Index));
        }

        private bool JobModelExists(int id)
        {
          return (_context.JobModel?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
