using System;
using System.Configuration;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using WEB_APPLICATION.Models;

namespace WEB_APPLICATION.Controllers
{
    public class AccountController : Controller
    {
        // ================================================================
        // REGISTRATION — Step 1: fill form - the form basically takes the data from the field and then it does the server side validation and then it checks if the email is already registered and then it checks if the username is valid and then it checks if the username is already taken and then it stores the pending registration in the session until the email is verified and then it generates and sends the verification code and then it redirects to the VerifyEmail action
     

        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registration(string firstName, string lastName,
                                         string userName, string password,
                                         string email, string role)
        {
            // Server-side validation
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                ViewBag.Error = "First name and last name are required.";
                return View();
            }

            if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ViewBag.Error = "Please enter a valid email address.";
                return View();
            }

            UserDAL userDal = new UserDAL();

            // Check if email is already registered
            if (userDal.FindByEmail(email) != null)
            {
                ViewBag.Error = "That email address is already registered. Please log in or use a different email.";
                return View();
            }

            if (!userDal.CheckValidCredentials(userName, password))
            {
                ViewBag.Error = "Invalid username or password. Username: 4-20 chars, letters/numbers/underscore, cannot start with digit. Password: must contain uppercase, lowercase, and a number.";
                return View();
            }

            if (userDal.UsernameExists(userName))
            {
                ViewBag.Error = "Username already taken. Please choose a different one.";
                return View();
            }

            // Store pending registration in session until email is verified
            Session["PendingFirstName"] = firstName;
            Session["PendingLastName"] = lastName;
            Session["PendingUserName"] = userName;
            Session["PendingPassword"] = password;
            Session["PendingEmail"] = email;
            Session["PendingRole"] = role;

            // Generate and send verification code
            string code = EmailService.GenerateCode();
            DateTime expiry = DateTime.Now.AddMinutes(10);

            EmailVerificationDAL verifyDal = new EmailVerificationDAL();
            verifyDal.SaveCode(email, code, expiry);

            try
            {
                EmailService.SendVerificationCode(email, code);
            }
            catch (Exception)
            {
                ViewBag.Error = "Could not send verification email. Please check the address and try again.";
                return View();
            }

            TempData["VerifyEmail"] = email;
            return RedirectToAction("VerifyEmail");
        }

        // ================================================================
        // REGISTRATION — Step 2: verify email code
        // ================================================================

        [HttpGet]
        public ActionResult VerifyEmail()
        {
            if (TempData["VerifyEmail"] == null && Session["PendingEmail"] == null)
                return RedirectToAction("Registration");

            ViewBag.Email = TempData["VerifyEmail"] ?? Session["PendingEmail"];

            if (TempData["ResendSuccess"] != null)
                ViewBag.ResendSuccess = TempData["ResendSuccess"];

            return View();
        }

        [HttpPost]
        public ActionResult VerifyEmail(string code)
        {
            string email = Session["PendingEmail"] as string;
            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Session expired. Please register again.";
                return RedirectToAction("Registration");
            }

            EmailVerificationDAL verifyDal = new EmailVerificationDAL();
            bool valid = verifyDal.VerifyCode(email, code?.Trim());

            if (!valid)
            {
                ViewBag.Error = "Invalid or expired code. Please try again or request a new one.";
                ViewBag.Email = email;
                return View();
            }

            // Code verified — create the account
            string firstName = Session["PendingFirstName"] as string;
            string lastName = Session["PendingLastName"] as string;
            string userName = Session["PendingUserName"] as string;
            string password = Session["PendingPassword"] as string;
            string roleStr = (Session["PendingRole"] as string) ?? "student";
            User.Role userRole = UtilityDAL.parseStringToRole(roleStr.ToLower());

            UserDAL userDal = new UserDAL();
            int result = userDal.RegisterUser(userName, password, userRole, firstName, lastName, email);

            // Clear pending session keys regardless of outcome
            Session.Remove("PendingFirstName");
            Session.Remove("PendingLastName");
            Session.Remove("PendingUserName");
            Session.Remove("PendingPassword");
            Session.Remove("PendingEmail");
            Session.Remove("PendingRole");

            if (result == 0)
            {
                TempData["success"] = "Account created! Welcome to EduNest.";
                return RedirectToAction("Login");
            }
            else if (result == 2627)
            {
                TempData["Error"] = "Username was taken while you were verifying. Please register again with a different username.";
                return RedirectToAction("Registration");
            }
            else
            {
                ViewBag.Error = "Registration failed (error " + result + "). Please try again.";
                ViewBag.Email = email;
                return View();
            }
        }

        [HttpPost]
        public ActionResult ResendCode()
        {
            string email = Session["PendingEmail"] as string;
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Registration");

            string code = EmailService.GenerateCode();
            DateTime expiry = DateTime.Now.AddMinutes(10);

            EmailVerificationDAL verifyDal = new EmailVerificationDAL();
            verifyDal.SaveCode(email, code, expiry);

            try { EmailService.SendVerificationCode(email, code); }
            catch (Exception) { /* best-effort */ }

            TempData["VerifyEmail"] = email;
            TempData["ResendSuccess"] = "A new code was sent to " + email;
            return RedirectToAction("VerifyEmail");
        }

        // ================================================================
        // LOGIN — standard username/password
        // ================================================================

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string userName, string password)
        {
            UserDAL userDal = new UserDAL();
            SessionDAL sessionDal = new SessionDAL();
            int authResult = userDal.LoginAuthentication(userName, password);

            if (authResult == 0)
            {
                User user = userDal.GetUserByUsername(userName);
                if (user != null)
                {
                    SetSessionForUser(user, sessionDal);
                    TempData["success"] = "Login successful!";
                    return RedirectByRole(user.role);
                }
            }

            if (authResult == 2)       ViewBag.Error = "Username not found.";
            else if (authResult == 1)  ViewBag.Error = "Incorrect password.";
            else if (authResult == 3)  ViewBag.Error = "Your account has been deactivated.";
            else if (authResult == 4)  ViewBag.Error = "This account uses Google Sign-In. Please click the Google button below.";
            else                       ViewBag.Error = "Login failed. Please try again.";

            return View();
        }

        // ================================================================
        // GOOGLE SIGN-IN
        // ================================================================

        /// <summary>
        /// Called via form POST from the Google Identity Services JS library.
        /// Validates the signed JWT 'credential', then logs the user in or
        /// starts the role-selection flow for new Google users.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> GoogleCallback(string credential)
        {
            if (string.IsNullOrEmpty(credential))
                return RedirectToAction("Login");

            // Validate the JWT by calling Google's tokeninfo endpoint
            JObject payload;
            try
            {
                using (HttpClient http = new HttpClient())
                {
                    string url = "https://oauth2.googleapis.com/tokeninfo?id_token=" + credential;
                    string json = await http.GetStringAsync(url);
                    payload = JObject.Parse(json);
                }
            }
            catch (Exception)
            {
                TempData["GoogleError"] = "Google sign-in failed. Please try again.";
                return RedirectToAction("Login");
            }

            // Reject tokens meant for a different application
            string expectedClientId = ConfigurationManager.AppSettings["Google:ClientId"];
            string aud = payload["aud"]?.ToString();
            if (aud != expectedClientId || payload["error"] != null)
            {
                TempData["GoogleError"] = "Invalid Google token. Please try again.";
                return RedirectToAction("Login");
            }

            string googleId = payload["sub"]?.ToString();
            string email = payload["email"]?.ToString();
            string firstName = payload["given_name"]?.ToString() ?? "";
            string lastName = payload["family_name"]?.ToString() ?? "";

            if (string.IsNullOrEmpty(googleId) || string.IsNullOrEmpty(email))
            {
                TempData["GoogleError"] = "Could not retrieve your Google account info. Please try again.";
                return RedirectToAction("Login");
            }

            UserDAL userDal = new UserDAL();
            SessionDAL sessionDal = new SessionDAL();

            // Case 1: returning Google user
            User existing = userDal.FindByGoogleId(googleId);
            if (existing != null)
            {
                SetSessionForUser(existing, sessionDal);
                TempData["success"] = "Welcome back, " + existing.firstName + "!";
                return RedirectByRole(existing.role);
            }

            // Case 2: email already in the system (registered traditionally)
            User emailMatch = userDal.FindByEmail(email);
            if (emailMatch != null)
            {
                SetSessionForUser(emailMatch, sessionDal);
                TempData["success"] = "Signed in with Google.";
                return RedirectByRole(emailMatch.role);
            }

            // Case 3: brand new user — collect role before creating account
            Session["GoogleId"] = googleId;
            Session["GoogleEmail"] = email;
            Session["GoogleFirstName"] = firstName;
            Session["GoogleLastName"] = lastName;

            return RedirectToAction("CompleteGoogleProfile");
        }

        // ================================================================
        // GOOGLE — role selection (new Google users only)
        // ================================================================

        [HttpGet]
        public ActionResult CompleteGoogleProfile()
        {
            if (Session["GoogleId"] == null)
                return RedirectToAction("Login");

            ViewBag.FirstName = Session["GoogleFirstName"];
            ViewBag.Email = Session["GoogleEmail"];
            return View();
        }

        [HttpPost]
        public ActionResult CompleteGoogleProfile(string role)
        {
            string googleId = Session["GoogleId"] as string;
            string email = Session["GoogleEmail"] as string;
            string firstName = Session["GoogleFirstName"] as string ?? "";
            string lastName = Session["GoogleLastName"] as string ?? "";

            if (string.IsNullOrEmpty(googleId))
                return RedirectToAction("Login");

            User.Role userRole = UtilityDAL.parseStringToRole((role ?? "student").ToLower());
            string autoUsername = GenerateUniqueUsername(email, firstName, lastName);

            UserDAL userDal = new UserDAL();
            int newUserId = userDal.RegisterGoogleUser(googleId, email, firstName, lastName, userRole, autoUsername);

            if (newUserId < 0)
            {
                ViewBag.Error = "Account creation failed. Please try again.";
                ViewBag.FirstName = firstName;
                ViewBag.Email = email;
                return View();
            }

            // Clean up temp Google session keys
            Session.Remove("GoogleId");
            Session.Remove("GoogleEmail");
            Session.Remove("GoogleFirstName");
            Session.Remove("GoogleLastName");

            // Send welcome email (best-effort — don't block login if it fails)
            try { EmailService.SendWelcomeEmail(email, firstName, role ?? "student"); }
            catch (Exception) { }

            // Log the new user straight in
            User newUser = userDal.GetUserById(newUserId);
            if (newUser != null)
            {
                SessionDAL sessionDal = new SessionDAL();
                SetSessionForUser(newUser, sessionDal);
                TempData["success"] = "Welcome to EduNest, " + firstName + "!";
                return RedirectByRole(newUser.role);
            }

            TempData["success"] = "Account created! Please sign in.";
            return RedirectToAction("Login");
        }

        // ================================================================
        // LOGOUT
        // ================================================================

        public ActionResult Logout()
        {
            if (Session["sessionId"] != null)
            {
                SessionDAL sessionDal = new SessionDAL();
                sessionDal.LogLogout((int)Session["sessionId"]);
            }
            Session.Clear();
            Session.Abandon();
            TempData["success"] = "You have been logged out.";
            return RedirectToAction("Index", "Home");
        }

        // ================================================================
        // Private helpers
        // ================================================================

        private void SetSessionForUser(User user, SessionDAL sessionDal)
        {
            Session["userId"] = user.userId;
            Session["userName"] = user.userName;
            Session["firstName"] = user.firstName;
            Session["role"] = user.role.ToString().ToLower();
            int sessionId = sessionDal.LogLogin(user.userId);
            Session["sessionId"] = sessionId;
        }

        private ActionResult RedirectByRole(Models.User.Role role)
        {
            if (role == Models.User.Role.Admin)
                return RedirectToAction("Dashboard", "Admin");
            if (role == Models.User.Role.Instructor)
                return RedirectToAction("Dashboard", "Instructor");
            return RedirectToAction("MyEnrollments", "Enrollment");
        }

        /// <summary>
        /// Generates a valid, unique username from a Google user's email address.
        /// Falls back to first+last name if the email local part is unusable.
        /// </summary>
        private string GenerateUniqueUsername(string email, string firstName, string lastName)
        {
            UserDAL userDal = new UserDAL();

            // Strip @domain and all non-alphanumeric characters
            string localPart = email.Split('@')[0];
            string clean = Regex.Replace(localPart, @"[^a-zA-Z0-9]", "").ToLower();

            // Cannot start with a digit
            if (clean.Length == 0 || char.IsDigit(clean[0]))
                clean = "u" + clean;

            // Pad if too short
            if (clean.Length < 4)
                clean = Regex.Replace((firstName + lastName).ToLower(), @"[^a-zA-Z0-9]", "") + clean;
            if (clean.Length < 4)
                clean = "user" + clean;

            // Keep within 18 chars to allow a 2-digit suffix
            if (clean.Length > 18) clean = clean.Substring(0, 18);

            string candidate = clean;
            int suffix = 1;
            while (userDal.UsernameExists(candidate))
            {
                candidate = clean + suffix;
                suffix++;
            }

            return candidate;
        }
    }
}
