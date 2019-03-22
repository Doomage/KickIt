﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using TeamProject.Models;
using TeamProject.ModelsViews;


namespace TeamProject.ApiControllers
{
    [Authorize]
    public class BookController : ApiController
    {
        private ProjectDbContext db = new ProjectDbContext();

        // GET: api/Book/5
        public IEnumerable<TimeslotApiView> Get(int id, DateTime fromDate, DateTime toDate)
        {
            return db.TimeSlots.GetForBooking(id, fromDate, toDate);
        }

        // POST: api/Book
        [HttpPost]
        public PostResponse Post(PutBookModel putBookModel)
        {
            if (!GetLoggedInUserId(out int loggedUserId))
            {
                return new PostResponse() { Status = "You must log in first" };
            }

            if (!IsValidDate(putBookModel.BookedAt))
            {
                return new PostResponse() { Status = $"Can't Book on date before {DateTime.Now.Date}" };
            }

            var booking = new Booking()
            {
                CourtId = putBookModel.CourtId,
                BookedAt = putBookModel.BookedAt.ToLocalTime(),
                UserId = loggedUserId,
                BookKey = Guid.NewGuid().ToString("N"),
                Duration = 60
            };

            booking = db.Bookings.Add(booking);

            if (booking.Id == 0)
            {
                return new PostResponse() { Status = $"At <small class='text-muted'>{putBookModel.BookedAt.ToLocalTime()}</small>" };
            }

            return new PostResponse()
            {
                Status = db.LastActionStatus,
                BookingId = booking.Id,
                BookKey = booking.BookKey
            };
        }
        private bool IsValidDate(DateTime BookedAt)
        {
            return DateTime.Compare(BookedAt.Date, DateTime.Now.Date) >= 0;
        }
        private bool GetLoggedInUserId(out int loggedUserId)
        {
            var identity = User.Identity as ClaimsIdentity;
            var userDateId = identity.FindFirst(c => c.Type == ClaimTypes.UserData).Value;

            return int.TryParse(userDateId, out loggedUserId);
        }
        public class PostResponse
        {
            public string Status { get; set; }
            public int BookingId { get; set; }
            public string BookKey { get; set; }
        }
    }

}
