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
using Munkanaplo2.Global;

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
		[Authorize]
		public async Task<IActionResult> Index([Bind("Id")] int Id)
		{
			//if (TeacherHelper.IsTeacher(User)) return RedirectToAction("TeacherView");
			var projectMemberships = _context.ProjectMemberships
									.Where(pm => pm.ProjectId == Id)
									.ToList();

			if (!projectMemberships.Where(pm => pm.Member == User.Identity.Name).Any())
			{
				return View("AccesDenied");
			}

			ViewBag.ProjectId = Id;
			ViewBag.UnFinishedJobs = await _context.JobModel.Where(jm => jm.ProjectId == Id && jm.JobStatus == "folyamatban").ToListAsync();
			ViewBag.FinishedJobs = await _context.JobModel.Where(jm => jm.ProjectId == Id && jm.JobStatus != "folyamatban").ToListAsync();
			return View("Index");
		}

		/*[Authorize]
		public async Task<IActionResult> TeacherView([Bind("Id")] int Id)
		{
			if (TeacherHelper.IsTeacher(User)) return RedirectToAction("Index");
			var projectMemberships = _context.ProjectMemberships
									.Where(pm => pm.ProjectId == Id)
									.ToList();

			if (!projectMemberships.Where(pm => pm.Member == User.Identity.Name).Any())
			{
				return View("AccesDenied");
			}
			ViewBag.ProjectId = Id;
			ViewBag.UnFinishedJobs = await _context.JobModel.Where(jm => jm.ProjectId == Id && jm.JobStatus == "folyamatban").ToListAsync();
			ViewBag.FinishedJobs = await _context.JobModel.Where(jm => jm.ProjectId == Id && jm.JobStatus != "folyamatban").ToListAsync();
			return View();
		}*/

		// GET: Jobs/Details/5
		[Authorize]
		public async Task<IActionResult> Details(int id)
		{
			if (id == null || _context.JobModel == null)
			{
				return NotFound();
			}

			var jobModel = await _context.JobModel
				.FirstOrDefaultAsync(m => m.Id == id);
			var subTasks = await _context.SubTaskModel
				.Where(m => m.JobId == id).ToListAsync();

			var projectMemberships = _context.ProjectMemberships
									.Where(pm => pm.ProjectId == jobModel.ProjectId)
									.ToList();

			if (!projectMemberships.Where(pm => pm.Member == User.Identity.Name).Any())
			{
				return View("AccesDenied");
			}

			ViewBag.ProjectId = jobModel.ProjectId;
			ViewBag.SubTasks = subTasks;

			if (jobModel == null)
			{
				return NotFound();
			}

			return View(jobModel);
		}


		// GET: Jobs/Create
		[Authorize]
		public async Task<IActionResult> Create([Bind("Id")] int Id)
		{
			if (TeacherHelper.IsTeacher(User)) return View("AccesDenied");
			var projectMemberships = _context.ProjectMemberships
									.Where(pm => pm.ProjectId == Id)
									.ToList();

			if (!projectMemberships.Where(pm => pm.Member == User.Identity.Name).Any())
			{
				return View("AccesDenied");
			}

			List<string> users = new List<string>();
			var usersInProject = await _context.ProjectMemberships.Where(pm => pm.ProjectId == Id).ToListAsync();
			foreach (ProjectMembership pm in usersInProject)
			{
				if (!TeacherHelper.IsTeacher(pm.Member)) users.Add(pm.Member);
			}
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
			if (TeacherHelper.IsTeacher(User)) return View("AccesDenied");
			var projectMemberships = _context.ProjectMemberships
									.Where(pm => pm.ProjectId == jobModel.ProjectId)
									.ToList();

			if (!projectMemberships.Where(pm => pm.Member == User.Identity.Name).Any())
			{
				return View("AccesDenied");
			}

			if (ModelState.IsValid)
			{
				_context.Add(jobModel);
				await _context.SaveChangesAsync();

				ViewBag.ProjectId = jobModel.ProjectId;
				return LocalRedirect("/" + jobModel.ProjectId + "/feladatok");
			}
			return View("Create");
		}

		[HttpPost()]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateSubTask([Bind("Id,TaskTitle,TaskDetails,JobId,TaskCreator,TaskCreationDate")] SubTaskModel subTaskModel)
		{
			if (TeacherHelper.IsTeacher(User)) return View("AccesDenied");

			var jobModel = await _context.JobModel.FindAsync(subTaskModel.JobId);
			var projectMemberships = _context.ProjectMemberships
									.Where(pm => pm.ProjectId == jobModel.ProjectId)
									.ToList();

			if (!projectMemberships.Where(pm => pm.Member == User.Identity.Name).Any())
			{
				return View("AccesDenied");
			}

			if (ModelState.IsValid)
			{
				_context.Add(subTaskModel);
				await _context.SaveChangesAsync();

				return LocalRedirect("/feladat/" + subTaskModel.JobId);
			}
			return View("Hiba");
		}

		[HttpPost()]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteSubTask([Bind("Id")] int id)
		{
			var subTask = await _context.SubTaskModel.FindAsync(id);
			if (subTask == null) return NotFound();

			if (TeacherHelper.IsTeacher(User)) return View("AccesDenied");

			var jobModel = await _context.JobModel.FindAsync(subTask.JobId);
			var projectMemberships = _context.ProjectMemberships
									.Where(pm => pm.ProjectId == jobModel.ProjectId)
									.ToList();

			if (!projectMemberships.Where(pm => pm.Member == User.Identity.Name).Any())
			{
				return View("AccesDenied");
			}

			_context.Remove(subTask);
			await _context.SaveChangesAsync();


			return LocalRedirect("/feladat/" + subTask.JobId);
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
			List<SubTaskModel> subTasks = await _context.SubTaskModel
					.Where(m => m.JobId == jobModel.Id).ToListAsync();

			if (TeacherHelper.IsTeacher(User)) return View("AccesDenied");
			var projectMemberships = _context.ProjectMemberships
									.Where(pm => pm.ProjectId == jobModel.ProjectId)
									.ToList();
			if (!projectMemberships.Where(pm => pm.Member == User.Identity.Name).Any())
			{
				return View("AccesDenied");
			}

			if (jobModel == null)
			{
				return NotFound();
			}

			List<string> users = new List<string>();
			var usersInProject = await _context.ProjectMemberships.Where(pm => pm.ProjectId == jobModel.ProjectId).ToListAsync();
			foreach (ProjectMembership pm in usersInProject)
			{
				users.Add(pm.Member);
			}
			ViewBag.Users = new SelectList(users);

			ViewBag.ProjectId = jobModel.ProjectId;
			ViewBag.SubTasks = subTasks;
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

			if (TeacherHelper.IsTeacher(User)) return View("AccesDenied");

			var projectMemberships = _context.ProjectMemberships
									.Where(pm => pm.ProjectId == jobModel.ProjectId)
									.ToList();

			if (!projectMemberships.Where(pm => pm.Member == User.Identity.Name).Any())
			{
				return View("AccesDenied");
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
				return LocalRedirect("/" + jobModel.ProjectId + "/feladatok");
			}
			return View("Hiba");
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
			if (TeacherHelper.IsTeacher(User)) return View("AccesDenied");
			var projectMemberships = _context.ProjectMemberships
									.Where(pm => pm.ProjectId == jobModel.ProjectId)
									.ToList();

			if ((jobModel.JobOwner != User.Identity.Name.ToString()) && (jobModel.JobCreator != User.Identity.Name.ToString()))
			{
				return View("AccesDenied");
			}


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
		public async Task<IActionResult> DeleteConfirmed([Bind("Id")] int id)
		{
			if (_context.JobModel == null)
			{
				return Problem("Entity set 'ApplicationDbContext.JobModel'  is null.");
			}
			List<SubTaskModel> subTasks = await _context.SubTaskModel.Where(stm => stm.JobId == id).ToListAsync();
			var jobModel = await _context.JobModel.FindAsync(id);

			if (TeacherHelper.IsTeacher(User)) return View("AccesDenied");
			var projectMemberships = _context.ProjectMemberships
									.Where(pm => pm.ProjectId == jobModel.ProjectId)
									.ToList();

			if ((jobModel.JobOwner != User.Identity.Name.ToString()) && (jobModel.JobCreator != User.Identity.Name.ToString()))
			{
				return View("AccesDenied");
			}

			foreach (SubTaskModel subTask in subTasks)
			{
				_context.SubTaskModel.Remove(subTask);
			}

			if (jobModel != null)
			{
				_context.JobModel.Remove(jobModel);
			}

			await _context.SaveChangesAsync();

			return LocalRedirect("/" + jobModel.ProjectId + "/feladatok");
		}

		private bool JobModelExists(int id)
		{
			return (_context.JobModel?.Any(e => e.Id == id)).GetValueOrDefault();
		}
	}
}
