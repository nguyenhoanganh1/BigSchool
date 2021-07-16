using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BigShool.DTO;
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

        [Authorize]
        public ActionResult Mine()
        {
            var userId = User.Identity.GetUserId();
            var course = db.Courses.Where(c => c.LecturerId == userId && c.DateTime > DateTime.Now)
                .Include(l => l.Lecturer)
                .Include(c => c.Category).ToList();
            return View(course);
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
                Categories = db.Categories.ToList(),
                Heading = "Add Course"
            };
            return View("CourseForm", viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CourseViewModel viewModel, HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    ModelState.Remove("LecturerId");
                    string _FileName = Path.GetFileName(file.FileName);
                    string _path = Path.Combine(Server.MapPath("~/Content/images"), _FileName);

                    file.SaveAs(_path);
                    var course = new Course
                    {
                        LecturerId = User.Identity.GetUserId(),
                        DateTime = viewModel.GetDateTime(),
                        CategoryId = viewModel.Category,
                        Place = viewModel.Place,
                        Images = _FileName
                    };

                    db.Courses.Add(course);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
                return View();
            }
            return View();
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

        [Authorize]
        public ActionResult Edit2(int id)
        {
            var userId = User.Identity.GetUserId();
            var course = db.Courses.Single(c => c.Id == id && c.LecturerId == userId);
            var viewModel = new CourseViewModel()
            {
                Categories = db.Categories.ToList(),
                Date = course.DateTime.ToString("dd/MM/yyyy"),
                Time = course.DateTime.ToString("HH:mm"),
                Category = course.CategoryId,
                Place = course.Place,
                Heading = "Edit Course",
                Id = course.Id
            };
            return View("CourseForm", viewModel);
        }

        [HttpPost]
        public ActionResult Update(CourseViewModel model, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = db.Categories.ToList();
                return View("Create", model);
            }
            var userId = User.Identity.GetUserId();
            var course = db.Courses.Single(c => c.Id == model.Id && c.LecturerId == userId);
            if (file != null)
            {
                var location = Server.MapPath("~/Content/images");
                if (!string.IsNullOrEmpty(course.Images))
                {

                    var existingFile = Path.Combine(location, course.Images);
                    if (System.IO.File.Exists(existingFile))
                    {
                        System.IO.File.Delete(existingFile);
                    }

                }
                string _FileName = Path.GetFileName(file.FileName);
                string _path = Path.Combine(location, _FileName);
                file.SaveAs(_path);
                course.Images = _FileName;
            }
            course.Place = model.Place;
            course.DateTime = model.GetDateTime();
            course.CategoryId = model.Category;

            db.SaveChanges();
            return RedirectToAction("Mine", "Courses");
        }

        public ActionResult Delete2(int id)
        {
            var course = db.Courses.Where(c => c.Id == id).FirstOrDefault();
            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Mine", "Courses");
        }

        public ActionResult Follow()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Followings.Where(a => a.FollowerId == userId).ToList();
            return View(user);
        }
    }

}
