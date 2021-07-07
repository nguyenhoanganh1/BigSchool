using BigShool.DTO;
using BigShool.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BigShool.Controllers
{
    public class FollowingsController : ApiController
    {
        private readonly ApplicationDbContext db;

        public FollowingsController()
        {
            db = new ApplicationDbContext();
        }

        [HttpPost]
        public IHttpActionResult Follow(FollowingDto followingDto)
        {
            var userId = User.Identity.GetUserId();
            if (db.Followings.Any(a => a.FollowerId == userId && a.FolloweeId == followingDto.FolloweeId))
            {
                return BadRequest("The Attendance already exists !");
            }
            var following = new Following
            {
                FollowerId = userId,
                FolloweeId = followingDto.FolloweeId
            };
            db.Followings.Add(following);
            db.SaveChanges();
            return Ok();
        }
    }
}
