using System;
using System.Collections.Generic;
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
        public ActionResult Registration() 
        {
            
            return View() ; 
        }

        [HttpPost] // post 
        public ActionResult Registration(string userName , string password , string role , string firstName , string lastName   )
        {
            // checking for valid credentials 
            UserDAL userDal = new UserDAL() ; 
            
            User.Role  userRole = UtilityDAL.parseStringToRole(role.ToLower());
            bool valid = userDal.CheckValidCredentials(userName , password ) ;
           
            if (!valid  ) // check if credentials are valid first ; 
            {
                ViewBag.Error = "The entered credentials are not vaild !";
                return View();
            } 
            else
            { // then attempt to register 
                bool success =  userDal.RegisterUser(userName , password, userRole , firstName , lastName ) ;
                if (success )
                {
                    TempData["success"] = "You have successfullly Registered into Edu Nest " ;
                    return RedirectToAction("Login","Account") ; // redirects it to the login page 
                } 
                else
                {
                    ViewBag.Error = "An issue occured while Attempting registration " ;     
                    return View() ; // returns to the registration page 
                }                
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
                User user = new UserDAL().GetUserByUsername(userName);
                if (user != null)
                {
                    Session["userId"] = user.userId;
                    Session["userName"] = user.userName;
                    Session["role"] = user.role.ToString().ToLower();
                   
                    int sessionId = sessoinDal.LogLogin(user.userId);
                    Session["sessionId"] = sessionId;  // storing the sessionId fo rlater logout purposs 
                    TempData["success"] = "Login successful!";
                    
                    if (user.role == WEB_APPLICATION.Models.User.Role.Admin)
                        return RedirectToAction("Dashboard", "Admin");
                    else if (user.role == WEB_APPLICATION.Models.User.Role.Instructor)
                        return RedirectToAction("Index", "Course");
                    else
                        return RedirectToAction("Index", "Home");
                }
            }
            
            if (authResult == 2)
                ViewBag.Error = "Username not found";
            else if (authResult == 1)
                ViewBag.Error = "Incorrect password";
            else
                ViewBag.Error = "Login failed";
            
            return View();
        }

        public ActionResult Logout()
        {
            SessionDAL sessionDal = new SessionDAL();
            sessionDal.LogLogout((int)Session["sessionId"]);
            Session.Clear();
            TempData["success"] = "You have logged out";
            return RedirectToAction("Index", "Home");
        }
    }
}
    
    
    
