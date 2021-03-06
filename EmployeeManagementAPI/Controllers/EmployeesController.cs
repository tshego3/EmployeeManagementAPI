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
using EmployeeManagementAPI.Data;
using EmployeeManagementAPI.Models;
using Microsoft.AspNet.Identity;

namespace EmployeeManagementAPI.Controllers
{
    [Authorize] //Method 1 - Authorize required on all the methods.
    public class EmployeesController : ApiController
    {
        private EmployeeContext db = new EmployeeContext();

        //***Update.

        // GET: api/Employees
        //[Authorize] //Method 2 - Authorize individual methods.
        public IQueryable<Employee> GetEmployees()
        {
            return db.Employees;
        }

        // GET: api/Employees/ForCurrentUser
        //***Display only the records created by current logged in user.
        [Route("api/Employees/ForCurrentUser")]
        public IQueryable<Employee> GetEmployeesForCurrentUser()
        {
            string userId = User.Identity.GetUserId();
            return db.Employees.Where(user => user.UserID == userId);
        }

        // GET: api/Employees/Search/Peter
        //***Display only the records containing the "keyword".
        [Route("api/Employees/Search/{keyword}")]
        [HttpGet]
        public IQueryable<Employee> SearchEmployees(string keyword)
        {
            return db.Employees.Where(employee => employee.tbFirstName.Contains(keyword) || employee.tbSurname.Contains(keyword));
        }

        // GET: api/Employees/5
        [ResponseType(typeof(Employee))]
        public IHttpActionResult GetEmployee(int id)
        {
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        // PUT: api/Employees/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEmployee(int id, Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != employee.employeeID)
            {
                return BadRequest();
            }

            //***Prevent records from being deleted by other users, which are not the record owner/creator.
            string userId = User.Identity.GetUserId();
            if (userId != employee.UserID)
            {
                return StatusCode(HttpStatusCode.Conflict);
            }

            db.Entry(employee).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
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

        // POST: api/Employees
        [ResponseType(typeof(Employee))]
        public IHttpActionResult PostEmployee(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //***Pass the current logged in user's "userID" to the created record.
            string userId = User.Identity.GetUserId();
            employee.UserID = userId;

            db.Employees.Add(employee);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = employee.employeeID }, employee);
        }

        // DELETE: api/Employees/5
        [ResponseType(typeof(Employee))]
        public IHttpActionResult DeleteEmployee(int id)
        {
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return NotFound();
            }

            //***Prevent records from being deleted by other users, which are not the record owner/creator.
            string userId = User.Identity.GetUserId();
            if (userId != employee.UserID)
            {
                return StatusCode(HttpStatusCode.Conflict);
            }

            db.Employees.Remove(employee);
            db.SaveChanges();

            return Ok(employee);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EmployeeExists(int id)
        {
            return db.Employees.Count(e => e.employeeID == id) > 0;
        }
    }
}