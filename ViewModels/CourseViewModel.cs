using BigShool.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BigShool.ViewModels
{
    public class CourseViewModel
    {
        public int Id { get; set; }
        public IEnumerable<Course> UpcommingCourses { get; set; }
        public bool ShowAction { get; set; }

        public string Images { get; set; }

        [Required]
        public string Place { get; set; }
        [Required]
        [FutureDate]
        public string Date { get; set; }
        [ValidTime]
        [Required]
        public string Time { get; set; }
        [Required]
        public byte Category { get; set; }

        public string Heading { get; set; }

        public string action
        {
            get { return (Id != 0) ? "Update" : "Create"; }
        }
        public IEnumerable<Category> Categories { get; set; }

        public DateTime GetDateTime()
        {
            return DateTime.ParseExact(Date + " " + Time, "dd/MM/yyyy HH:mm", null);
        }

    }
}