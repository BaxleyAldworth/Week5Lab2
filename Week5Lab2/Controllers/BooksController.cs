using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Week5Lab2.Models;

namespace Week5Lab2.Controllers
{
    public class BooksController : ApiController
    {
        private Library db = new Library();

        // GET: api/Books
        public IQueryable<Book> GetBooks()
        {
            return db.Books;
        }

        // GET: api/Books/5
        [ResponseType(typeof(Book))]
        public IHttpActionResult GetBook(int id)
        {
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        // PUT: api/Books/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutBook(int id, Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != book.Id)
            {
                return BadRequest();
            }

            db.Entry(book).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Books
        [ResponseType(typeof(Book))]
        public IHttpActionResult PostBook(Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Books.Add(book);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = book.Id }, book);
        }

        /* Don't want users to be able to delete books
        // DELETE: api/Books/5
        [ResponseType(typeof(Book))]
        public IHttpActionResult DeleteBook(int id)
        {
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return NotFound();
            }

            db.Books.Remove(book);
            db.SaveChanges();

            return Ok(book);
        }
        */

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BookExists(int id)
        {
            return db.Books.Count(e => e.Id == id) > 0;
        }

        //does the route work and does it touch what you want in the db
        [Route("api/books/checkout/{BookID}")]
        public IHttpActionResult CheckOut(CheckOutInfo info )
        { 
            Book book = db.Books.Find(info.BookID);
            if (book == null)
            {
                return NotFound();
            }
            //do same for student

            Student student = db.Students.Find(info.StudentID);
            if (student == null)
            {
                return NotFound();
            }

            //make sure not already checked out

            var isAlreadyCheckedOut = book.CheckOuts.Any(x => x.ActualReturnDate != null);

            if (isAlreadyCheckedOut)
            {
                return BadRequest("Book is already checked out.");
            }
            CheckOut cob = new CheckOut();
            cob.Book = book;
            cob.Student = student;
            cob.CheckOutDate = DateTime.Now;
            cob.ExpectedReturnDate = cob.CheckOutDate.GetValueOrDefault().AddMonths(1);

            db.SaveChanges();

            return Ok(cob); 
        }



        [Route("api/books/checkout/{BookID}")]
        [HttpPost]
        public IHttpActionResult CheckIn(int BookID)
        {
            Book book = db.Books.Find(BookID);
            if (book == null)
            {
                return NotFound();
            }

            var alreadyCheckedOutRow = book.CheckOuts.FirstOrDefault(x => x.ActualReturnDate != null);

            if (alreadyCheckedOutRow == null)
            {
                return BadRequest("Book is not checked out.");
            }

            alreadyCheckedOutRow.ActualReturnDate = DateTime.Now;

            db.SaveChanges();

            return Ok(alreadyCheckedOutRow);
        }


    }
}