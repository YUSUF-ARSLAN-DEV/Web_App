using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_APPLICATION.Models;


namespace WEB_APPLICATION.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        [HttpGet]
        public ActionResult Registration()  // this returns back the registration page 
        {
            
            return View() ; 
        }

        [HttpPost] // post   
        public ActionResult Registration(string userName, string password, string role, string firstName, string lastName)
        {
            UserDAL userDal = new UserDAL();
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName)) // server side validation checking if first  name and last name are left emty or not 
            {
                ViewBag.Error = "First name and last name are required.";
                return View();
            }
            User.Role userRole = UtilityDAL.parseStringToRole(role.ToLower());
            bool valid = userDal.CheckValidCredentials(userName, password);

            if (!valid)
            {
                ViewBag.Error = "The entered credentials are not valid!";
                return View();
            }

            int result = userDal.RegisterUser(userName, password, userRole, firstName, lastName);

            if (result == 0)
            {
                TempData["success"] = "You have successfully registered into EduNest!";
                return RedirectToAction("Login", "Account");
            }
            else if (result == 2627)
            {
                ViewBag.Error = "Username already exists. Please choose a different one.";
                return View();
            }
            else if (result == 8152)
            {
                ViewBag.Error = "One of your fields is too long. Please shorten and try again.";
                return View();
            }
            else
            {
                ViewBag.Error = "Registration failed. Error code: " + result;
                return View();
            }
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string userName, string password)
        {
            UserDAL userDal = new UserDAL();
            SessionDAL sessoinDal = new SessionDAL() ; 
            int authResult = userDal.LoginAuthentication(userName, password);
            
            if (authResult == 0) // success
            {
               User user =  userDal.GetUserByUsername(userName);
                if (user != null)
                {
                    Session["userId"] = user.userId;
                    Session["userName"] = user.userName;
                    Session["firstName"] = user.firstName;
                    Session["role"] = user.role.ToString().ToLower();
                   
                    int sessionId = sessoinDal.LogLogin(user.userId);
                    Session["sessionId"] = sessionId;  // storing the sessionId fo rlater logout purposs 
                    TempData["success"] = "Login successful!";
                    
                    if (user.role == WEB_APPLICATION.Models.User.Role.Admin)
                        return RedirectToAction("Dashboard", "Admin");
                    else if (user.role == WEB_APPLICATION.Models.User.Role.Instructor)
                        return RedirectToAction("Dashboard", "Instructor");
                    else
                        return RedirectToAction("MyEnrollments", "Enrollment");
                }
            }
            
            if (authResult == 2)
                ViewBag.Error = "Username not found";
            else if (authResult == 1)
                ViewBag.Error = "Incorrect password";
            else if (authResult == 3)
                ViewBag.Error = "Your account has been deactivated.";
            else
                ViewBag.Error = "Login failed";
            
            return View();
        }

    public ActionResult Logout()
    {
        if (Session["sessionId"] != null) // ensures user can not jump to login page without asession being created 
        {
            SessionDAL sessionDal = new SessionDAL();
            sessionDal.LogLogout((int)Session["sessionId"]);
        }
        Session.Clear();
            Session.Abandon(); // this abandons the session and clears all session data to ensure user is logged out 
            TempData["success"] = "You have logged out";
        return RedirectToAction("Index", "Home");
    }
    }
}
    
    
    
