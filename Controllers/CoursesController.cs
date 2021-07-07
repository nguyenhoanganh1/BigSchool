using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BigShool.Models;
using BigShool.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace BigShool.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext db;

        public CoursesController()
        {
            db = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            var viewModel = db.Courses.ToList();
            return View(viewModel);
        }


        // GET: Courses
        [Authorize]
        public ActionResult Create()
        {
            var viewModel = new CourseViewModel
            {
                Categories = db.Categories.ToList()
            };
            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CourseViewModel viewModel)
        {
            ModelState.Remove("LecturerId");
            if (!ModelState.IsValid)
            {
                viewModel.Categories = db.Categories.ToList();
                return View("Create", viewModel);
            }

            var course = new Course
            {
                LecturerId = User.Identity.GetUserId(),
                DateTime = viewModel.GetDateTime(),
                CategoryId = viewModel.Category,
                Place = viewModel.Place
            };
            db.Courses.Add(course);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult Attending()
        {
            var userId = User.Identity.GetUserId();

            var courses = db.Attendances
                    .Where(a => a.AttendeeId == userId)
                    .Select(a => a.Course)
                    .Include(a => a.Lecturer)
                    .Include(a => a.Category)
                    .ToList();
            var viewModel = new CourseViewModel
            {
                UpcommingCourses = courses,
                ShowAction = User.Identity.IsAuthenticated
            };
            return View(viewModel);
        }

    }

}
