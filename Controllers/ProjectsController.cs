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
using Microsoft.AspNetCore.Identity;
using Munkanaplo2.Global;

namespace Munkanaplo2.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Projects
        [Authorize]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.Name != null)
            {
                var projectMemberships = _context.ProjectMemberships.ToList();
                ViewBag.ProjectMemberships = projectMemberships;

                return _context.ProjectModel != null ?
                            View(await _context.ProjectModel.Where(pm => pm.ProjectMembers.Where(m => m.Member == User.Identity.Name.ToString()).Any()).ToListAsync()) :
                            Problem("Entity set 'ApplicationDbContext.ProjectModel'  is null.");

            }
            /*else if (User.Identity.Name != null && TeacherHelper.IsTeacher(User.Identity.Name))
            {
                return RedirectToAction("TeacherView");
            }*/
            else
            {
                return View("Hiba");
            }
        }

        /*[Authorize]
        public async Task<IActionResult> TeacherView()
        {
            if (User.Identity.Name != null && TeacherHelper.IsTeacher(User.Identity.Name))
            {
                var projectMemberships = _context.ProjectMemberships.ToList();
                ViewBag.ProjectMemberships = projectMemberships;

                return _context.ProjectModel != null ?
                            View(await _context.ProjectModel.Where(pm => pm.ProjectMembers.Where(m => m.Member == User.Identity.Name.ToString()).Any()).ToListAsync()) :
                            Problem("Entity set 'ApplicationDbContext.ProjectModel'  is null.");

            }
            else if (User.Identity.Name != null && !TeacherHelper.IsTeacher(User.Identity.Name))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View("Hiba");
            }
        }*/

        // GET: Projects/Details/5
        [Authorize]
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

            if (!projectMemberships.Where(pm => pm.Member == User.Identity.Name).Any())
            {
                return View("AccesDenied");
            }
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
            if (!TeacherHelper.IsTeacher(User)) return View();
            else return View("AccesDenied");
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
            if (TeacherHelper.IsTeacher(User)) return View("AccesDenied");

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
            if (TeacherHelper.IsTeacher(User)) return View("AccesDenied");

            if (id == null || _context.ProjectModel == null)
            {
                return NotFound();
            }

            var ProjectModel = await _context.ProjectModel.FindAsync(id);
            if (ProjectModel == null)
            {
                return NotFound();
            }

            if (ProjectModel.ProjectCreator != User.Identity.Name.ToString())
            {
                return View("AccesDenied");
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

            var projectMemberships = _context.ProjectMemberships
                        .Where(pm => pm.ProjectId == id)
                        .ToList();

            if (TeacherHelper.IsTeacher(User)) return View("AccesDenied");
            if (!projectMemberships.Where(pm => pm.Member == User.Identity.Name).Any())
            {
                return View("AccesDenied");
            }

            if (ProjectModel.ProjectCreator != null && ProjectModel.ProjectTitle != null)
            {

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

            var projectMemberships = _context.ProjectMemberships
                                    .Where(pm => pm.ProjectId == id)
                                    .ToList();

            if (!projectMemberships.Where(pm => pm.Member == User.Identity.Name).Any())
            {
                return View("AccesDenied");
            }

            if (ProjectModel.ProjectCreator != null && ProjectModel.ProjectTitle != null && projectMembersToRemove != string.Empty)
            {

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
            return View("Hiba");
        }

        /*[HttpPost]
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
        }*/

        [Authorize]
        public async Task<IActionResult> EditProjectMembers(int? id)
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
            var projectMemberships = _context.ProjectMemberships
                                    .Where(pm => pm.ProjectId == ProjectModel.Id)
                                    .ToList();

            if (TeacherHelper.IsTeacher(User)) return View("AccesDenied");
            if (!projectMemberships.Where(pm => pm.Member == User.Identity.Name).Any())
            {
                return View("AccesDenied");
            }

            if (ProjectModel.ProjectCreator != User.Identity.Name.ToString()) return View("AccesDenied");

            ViewBag.ProjectCreator = ProjectModel.ProjectCreator;
            ViewBag.ProjectId = ProjectModel.Id;

            List<ProjectMembership> memberships = await _context.ProjectMemberships.Where(pm => pm.ProjectId == ProjectModel.Id).ToListAsync();
            var users = await _context.Users.ToListAsync();

            List<EditUsersViewModel> viewModels = new List<EditUsersViewModel>();
            foreach (ProjectMembership membership in memberships)
            {
                if (membership.Member != ProjectModel.ProjectCreator)
                {
                    EditUsersViewModel viewModel = new EditUsersViewModel
                    {
                        UserName = membership.Member,
                        IsAdded = true
                    };
                    viewModels.Add(viewModel);

                }
            }
            foreach (var user in users)
            {
                if (!viewModels.Where(vm => vm.UserName == user.UserName).Any() && user.UserName != ProjectModel.ProjectCreator)
                {
                    EditUsersViewModel viewModel = new EditUsersViewModel
                    {
                        UserName = user.UserName,
                        IsAdded = false
                    };
                    viewModels.Add(viewModel);

                }
            }

            return View("EditUsers", viewModels);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProjectMembersConfirmed(int ProjectId, List<EditUsersViewModel> editUsersViewModels)
        {
            ProjectModel project = await _context.ProjectModel.FindAsync(ProjectId);

            var projectMemberships = _context.ProjectMemberships
                                    .Where(pm => pm.ProjectId == ProjectId)
                                    .ToList();

            if (TeacherHelper.IsTeacher(User)) return View("AccesDenied");
            if (!projectMemberships.Where(pm => pm.Member == User.Identity.Name).Any())
            {
                return View("AccesDenied");
            }
            if (project.ProjectCreator != User.Identity.Name.ToString()) return View("AccesDenied");

            if (project != null)
            {
                List<ProjectMembership> memberships = await _context.ProjectMemberships.Where(pm => pm.ProjectId == ProjectId).ToListAsync();
                foreach (EditUsersViewModel item in editUsersViewModels)
                {
                    ProjectMembership correspondingMembership = null;
                    foreach (ProjectMembership membership in memberships)
                    {
                        if (membership.Member == item.UserName)
                        {
                            correspondingMembership = membership;
                            break;
                        }
                    }

                    if (correspondingMembership != null && item.IsAdded == true) { }
                    else if (correspondingMembership == null && item.IsAdded == true) AddMember(item.UserName, ProjectId);
                    else if (correspondingMembership != null && item.IsAdded == false) RemoveMember(item.UserName, ProjectId);
                    else if (correspondingMembership == null && item.IsAdded == false) { }
                }

                return LocalRedirect("/projektek/" + project.Id);
            }
            return LocalRedirect("/hiba");
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

            if (TeacherHelper.IsTeacher(User)) return View("AccesDenied");
            if (ProjectModel.ProjectCreator != User.Identity.Name.ToString())
            {
                return View("AccesDenied");
            }


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

            if (TeacherHelper.IsTeacher(User)) return View("AccesDenied");
            if (ProjectModel.ProjectCreator != User.Identity.Name.ToString())
            {
                return View("AccesDenied");
            }

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

        void AddMember(string name, int projectId)
        {
            var project = _context.ProjectModel
                .Include(p => p.ProjectMembers)
                .FirstOrDefault(p => p.Id == projectId);

            if (project != null && project.ProjectCreator != null && project.ProjectTitle != null && name != string.Empty)
            {
                var projectMembershipToAdd = new ProjectMembership
                {
                    ProjectId = projectId,
                    Member = name
                };

                project.ProjectMembers.Add(projectMembershipToAdd);

                try
                {
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
        }

        void RemoveMember(string name, int projectId)
        {
            var project = _context.ProjectModel
                .Include(p => p.ProjectMembers)
                .FirstOrDefault(p => p.Id == projectId);

            if (project != null && name != string.Empty)
            {
                var projectMembershipToRemove = project.ProjectMembers
                    .FirstOrDefault(pm => pm.ProjectId == projectId && pm.Member == name);

                if (projectMembershipToRemove != null)
                {
                    project.ProjectMembers.Remove(projectMembershipToRemove);

                    try
                    {
                        _context.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        throw;
                    }
                }
            }
        }

    }
}
