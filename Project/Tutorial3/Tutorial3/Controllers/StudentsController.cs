using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Tutorial3.Models;

namespace Tutorial3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetStudents(string orderBy)
        {
            var students = new List<Student>();
            using(var sqlConnection = new SqlConnection(@"Server=db-mssql.pjwstk.edu.pl;Database=s18881;User Id=apbds18881;Password=admin;"))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = sqlConnection;
                    command.CommandText = "SELECT FirstName, LastName, BirthDate, Name, Semester " +
                                          "FROM Student, Enrollment, Study " +
                                          "WHERE Student.IdEnrollment = Enrollment.IdEnrollment" +
                                          "AND Study.IdStudy = Enrollment.IdStudy";
                    sqlConnection.Open();
                    var response = command.ExecuteReader();
                    while (response.Read())
                    {
                        var st = new Student
                        {
                            FirstName = response["FirstName"].ToString(),
                            LastName = response["LastName"].ToString(),
                            Studies = response["Studies"].ToString(),
                            BirthDate = DateTime.Parse(response["BirthDate"].ToString()),
                            Semester = int.Parse(response["Semester"].ToString())
                        };

                        students.Add(st);
                    }
                }
            }
            return Ok(students);
        }

        [HttpGet("{idStudent}")]
        public IActionResult GetStudent(string idStudent)
        {
            using (var sqlConnection =
                new SqlConnection(@"Server=db-mssql.pjwstk.edu.pl; Database=s18881; User ID=apbdss18881; Password=admin;"))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = sqlConnection;
                    command.CommandText = "SELECT  Semester " +
                                          "FROM Student, Enrollment " +
                                          "WHERE Student.IdEnrollment = Enrollment.IdEnrollment " +
                                          "AND IdStudent=@idStudent";
                    command.Parameters.AddWithValue("idStudent", idStudent);
                    sqlConnection.Open();
                    SqlDataReader dataReader = command.ExecuteReader();
                    if (dataReader.Read())
                        return Ok("Student(" + idStudent + ") studies on this " + Int32.Parse(dataReader["Semester"].ToString()) + " year");
                    return NotFound("Invalid Input Provided");
                }
            }
        }
    }
}
