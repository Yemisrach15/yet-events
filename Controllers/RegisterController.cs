using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ModelTrial.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModelTrial.Controllers
{
    public class RegisterController : Controller
    {
        public string ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=ModelTrialContext-da245adf-27ed-4c81-8611-a6e1abffab99;Trusted_Connection=True;MultipleActiveResultSets=true";
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult IndexPost(User user)
        {
            InsertIntoDb(user);

            return RedirectToAction("Index", "Login");
        }
        internal void InsertIntoDb(User user)
        {
            string queryString = "INSERT INTO dbo.Users VALUES (@Username, @Email, @Password)";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.Add("@Username", System.Data.SqlDbType.NVarChar).Value = user.Username;
                command.Parameters.Add("@Email", System.Data.SqlDbType.NVarChar).Value = user.Email;
                command.Parameters.Add("@Password", System.Data.SqlDbType.NVarChar).Value = user.Password;

                try
                {
                    connection.Open();

                    int rowsAffected = command.ExecuteNonQuery();

                    command.Dispose();
                    connection.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
