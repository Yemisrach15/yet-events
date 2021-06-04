using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ModelTrial.Data;
using ModelTrial.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ModelTrial.Controllers
{
    public class LoginController : Controller
    {
        private readonly ModelTrialContext _context;

        public string ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=ModelTrialContext-da245adf-27ed-4c81-8611-a6e1abffab99;Trusted_Connection=True;MultipleActiveResultSets=true";
        [TempData]
        public int UserId { get; set; }
        [TempData]
        public string UserName { get; set; }
        public LoginController(ModelTrialContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public User u;
        public IActionResult IndexPost(User user)
        {
            if (FindByUser(user)) 
            {
                u = _context.User.FromSqlRaw($"select * from Users where Username = '{user.Username}'").ToList()[0];
                TempData["Id"] = u.Id;
                UserId = u.Id;
                UserName = u.Username;

                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index");

            
        }
        public IActionResult Logout()
        {
            UserId = -1;
            UserName = null;
            return RedirectToAction("Index");
        }
        internal bool FindByUser(User user) 
        {
            bool success = false;

            string queryString = "SELECT * FROM dbo.Users WHERE Username = @Username AND Password = @Password";


            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.Add("@Username", System.Data.SqlDbType.NVarChar).Value = user.Username;
                command.Parameters.Add("@Password", System.Data.SqlDbType.NVarChar).Value = user.Password;

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        success = true;
                    }
                    else
                    {
                        success = false;
                    }

                    reader.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return success;
        }
    }

    public static class TempDataExtensions
    {
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            object o;
            tempData.TryGetValue(key, out o);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
        }
    }
}
