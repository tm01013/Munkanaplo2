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
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public delegate void ShowIndexHandler(int projectId, bool isTeacher);
        public static event ShowIndexHandler ShowIndex;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            var projectMemberships = _context.ProjectMemberships.ToList();
            ViewBag.ProjectMemberships = projectMemberships;

            return _context.ProjectModel != null ?
                        View(await _context.ProjectModel.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.ProjectModel'  is null.");
        }

        public async Task<IActionResult> TeacherView()
        {
            var projectMemberships = _context.ProjectMemberships.ToList();
            ViewBag.ProjectMemberships = projectMemberships;

            return _context.ProjectModel != null ?
                        View(await _context.ProjectModel.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.ProjectModel'  is null.");
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ProjectModel == null)
            {
                return NotFound();
            }

            var ProjectModel = await _context.ProjectModel
                .FirstOrDefaultAsync(m => m.Id == id);

            var projectMemberships = _context.ProjectMemberships
                        .Where(pm => pm.ProjectId == id)
                        .ToList();
            ViewBag.ProjectMemberships = projectMemberships;

            if (ProjectModel == null)
            {
                return NotFound();
            }

            return View(ProjectModel);
        }

        // GET: Projects/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost()]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectTitle, ProjectCreator")] ProjectModel ProjectModel)
        {
            //if (ModelState.IsValid)
            //{
            ProjectModel inputModel = ProjectModel;
            int projectId = 1;
            if (_context.ProjectModel.Any())
            {
                projectId = _context.ProjectModel.Max(p => p.Id) + 1;
            }
            inputModel.Id = projectId;

            var projectMemberships = new List<ProjectMembership>();
            var projectMembership = new ProjectMembership
            {
                ProjectId = projectId,
                Member = ProjectModel.ProjectCreator,
                Project = inputModel
            };
            projectMemberships.Add(projectMembership);

            inputModel.ProjectMembers = projectMemberships;


            var projectModelToAdd = new ProjectModel
            {
                Id = inputModel.Id,
                ProjectTitle = inputModel.ProjectTitle,
                ProjectCreator = inputModel.ProjectCreator,
                ProjectMembers = inputModel.ProjectMembers
            };

            _context.Add(projectModelToAdd);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            //}
            return View(ProjectModel);
        }

        [Authorize]
        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ProjectModel == null)
            {
                return NotFound();
            }

            var ProjectModel = await _context.ProjectModel.FindAsync(id);
            if (ProjectModel == null)
            {
                return NotFound();
            }
            var users = await _context.Users.ToListAsync();
            ViewBag.Users = new SelectList(users);
            return View(ProjectModel);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditConfirmed(int id, [Bind("Id, ProjectTitle, ProjectCreator")] ProjectModel ProjectModel)
        {
            if (id != ProjectModel.Id)
            {
                return NotFound();
            }

            if (ProjectModel.ProjectCreator != null && ProjectModel.ProjectTitle != null)
            {

                var projectMemberships = _context.ProjectMemberships
                    .Where(pm => pm.ProjectId == ProjectModel.Id)
                    .ToList();

                var projectModelToAdd = new ProjectModel
                {
                    Id = id,
                    ProjectTitle = ProjectModel.ProjectTitle,
                    ProjectCreator = ProjectModel.ProjectCreator,
                    ProjectMembers = projectMemberships
                };

                try
                {
                    _context.Update(projectModelToAdd);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectModelExists(ProjectModel.Id))
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
            return View("Edit", ProjectModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMembership(int id, [Bind("Id, ProjectTitle, ProjectCreator")] ProjectModel ProjectModel, [Bind("ProjectMembersToRemove")] string projectMembersToRemove)
        {
            if (id != ProjectModel.Id)
            {
                return NotFound();
            }

            if (ProjectModel.ProjectCreator != null && ProjectModel.ProjectTitle != null && projectMembersToRemove != string.Empty)
            {

                var projectMemberships = _context.ProjectMemberships
                    .Where(pm => pm.ProjectId == ProjectModel.Id)
                    .ToList();

                var projectMembershipToRemove = new ProjectMembership
                {
                    ProjectId = ProjectModel.Id,
                    Member = projectMembersToRemove
                };
                projectMemberships.Remove(projectMembershipToRemove);

                var projectModelToRemove = new ProjectModel
                {
                    Id = id,
                    ProjectTitle = ProjectModel.ProjectTitle,
                    ProjectCreator = ProjectModel.ProjectCreator,
                    ProjectMembers = projectMemberships
                };

                var projectMemberToRemove = _context.ProjectMemberships
                .FirstOrDefault(pm => pm.ProjectId == ProjectModel.Id && pm.Member == projectMembersToRemove);

                try
                {
                    _context.Update(projectModelToRemove);
                    if (projectMembersToRemove != null) _context.Remove(projectMemberToRemove);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectModelExists(ProjectModel.Id))
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
            return View("Error");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMembership(int id, [Bind("Id, ProjectTitle, ProjectCreator")] ProjectModel ProjectModel, [Bind("ProjectMembersToAdd")] string projectMembersToAdd)
        {
            if (id != ProjectModel.Id)
            {
                return NotFound();
            }

            if (ProjectModel.ProjectCreator != null && ProjectModel.ProjectTitle != null && projectMembersToAdd != string.Empty)
            {

                var projectMemberships = _context.ProjectMemberships
                    .Where(pm => pm.ProjectId == ProjectModel.Id)
                    .ToList();


                var projectMembershipToAdd = new ProjectMembership
                {
                    ProjectId = ProjectModel.Id,
                    Member = projectMembersToAdd
                };
                projectMemberships.Add(projectMembershipToAdd);

                var projectModelToAdd = new ProjectModel
                {
                    Id = id,
                    ProjectTitle = ProjectModel.ProjectTitle,
                    ProjectCreator = ProjectModel.ProjectCreator,
                    ProjectMembers = projectMemberships
                };

                try
                {
                    _context.Update(projectModelToAdd);
                    _context.Add(projectMembershipToAdd);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectModelExists(ProjectModel.Id))
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
            return View("Error");
        }

        [Authorize]
        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ProjectModel == null)
            {
                return NotFound();
            }

            var ProjectModel = await _context.ProjectModel
                .FirstOrDefaultAsync(m => m.Id == id);

            var projectMemberships = _context.ProjectMemberships
                        .Where(pm => pm.ProjectId == id)
                        .ToList();
            ViewBag.ProjectMemberships = projectMemberships;
            if (ProjectModel == null)
            {
                return NotFound();
            }

            return View(ProjectModel);
        }

        // POST: Projects/Delete/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProjectModel == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ProjectModel'  is null.");
            }
            var projectMemberships = _context.ProjectMemberships
                    .Where(pm => pm.ProjectId == id)
                    .ToList();
            var jobs = _context.JobModel.Where(jm => jm.ProjectId == id).ToList();

            var ProjectModel = await _context.ProjectModel.FindAsync(id);

            List<SubTaskModel> subTasks = new List<SubTaskModel>();
            foreach (var job in jobs)
            {
                foreach (var subTask in _context.SubTaskModel.Where(stm => stm.JobId == job.Id))
                {
                    subTasks.Add(subTask);
                }
            }

            if (ProjectModel != null)
            {
                foreach (ProjectMembership membership in projectMemberships)
                {
                    _context.Remove(membership);
                }
                foreach (JobModel job in jobs)
                {
                    _context.Remove(job);
                }
                foreach (SubTaskModel subTask in subTasks)
                {
                    _context.Remove(subTask);
                }
                _context.ProjectModel.Remove(ProjectModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectModelExists(int id)
        {
            return (_context.ProjectModel?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
