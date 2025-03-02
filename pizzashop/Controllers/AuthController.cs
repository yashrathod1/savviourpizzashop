using Microsoft.AspNetCore.Mvc;

using pizzashop_repository.ViewModels;

using pizzashop_service.Interface;

namespace pizzashop.Controllers;

public class AuthController : Controller
{

    private readonly IUserService _useService;
    private readonly IEmailSender _emailSender;

    public AuthController(IUserService userService , IEmailSender emailSender)
    {
        _useService = userService;
        _emailSender = emailSender;
    }

    [HttpGet]
    public IActionResult Login()
    {
        string req_cookie = Request.Cookies["UserEmail"];
        if (!string.IsNullOrEmpty(req_cookie))
        {
            return RedirectToAction("Index", "Dashboard");
        }
        return View();
    }

    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = _useService.AuthenicateUser(model.Email, model.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid Email or Password");
                return View(model);
            }

            var coockieopt = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            if (model.RememberMe)
            {
                coockieopt.Expires = DateTime.UtcNow.AddDays(30);
                Response.Cookies.Append("UserEmail", user.Email, coockieopt);
            }

            string token = _useService.GenerateJwttoken(user.Email, user.Roleid);

            Response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddHours(2)
            });

            TempData["success"] = "Login Successful";

            return RedirectToAction("Index", "Dashboard");

        }

        return View(model);
    }
    
    [HttpGet]

    public IActionResult ForgotPassword(string email)

    {

        if (!string.IsNullOrEmpty(email))

        {

            ViewData["Email"] = email;

        }

        else

        {

            ViewData["Email"] = "";

        }

        return View();

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ForgotPassword(ForgotViewModel objUser)
    {
        if (ModelState.IsValid)
        {

            string filePath = @"Y:/MainPizzashop/pizzashop/Template/EmailTemplate.html";
            string emailBody = System.IO.File.ReadAllText(filePath);

            var urii = Url.Action("ResetPassword", "Auth", new { email = objUser.Email }, Request.Scheme);
            emailBody = emailBody.Replace("{ResetLink}", urii);

            string subject = "Reset Password";
            _emailSender.SendEmailAsync(objUser.Email, subject, emailBody);

            TempData["success"] = "Password reset instructions have been sent to your email.";

        }
        return View(objUser);
    }

     [HttpGet]
    public IActionResult ResetPassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if(_useService.ResetPassword(model.Email, model.NewPassword, model.ConfirmPassword, out string message))
        {
            TempData["success"] = message;
            return RedirectToAction("Login", "Auth");
        }

        ModelState.AddModelError(string.Empty, message);
        return View(model);

       
    }
}
