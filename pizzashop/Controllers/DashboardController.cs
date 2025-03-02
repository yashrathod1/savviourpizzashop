using Microsoft.AspNetCore.Mvc;
using pizzashop_repository.ViewModels;
using pizzashop_service.Implementation;
using pizzashop_service.Interface;

namespace pizzashop.Controllers;

public class DashboardController : Controller
{
    private readonly IUserService _useService;

    public DashboardController(IUserService userService)
    {
        _useService = userService;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Profile()
    {
        var token = Request.Cookies["AuthToken"];
        var email = _useService.ExtractEmailFromToken(token);

        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login", "Login");
        }

        var model = _useService.GetUserProfile(email);

        if (model == null)
        {
            return NotFound("User Not Found");
        }

        ViewBag.Email = email;
        return View(model);
    }

    [HttpPost]
    public IActionResult Profile(UserTableViewModel model)
    {
        var token = Request.Cookies["AuthToken"];
        var email = _useService.ExtractEmailFromToken(token);

        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login", "Login");
        }

        var success = _useService.UpdateUserProfile(email, model);

        if (!success)
        {
            return NotFound("User Not Found");
        }

        TempData["success"] = "Profile updated successfully.";
        return View(model);
    }

    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ChangePassword(ChangePasswordViewModel model)
    {
        var token = Request.Cookies["AuthToken"];
        var userEmail = _useService.ExtractEmailFromToken(token);

        if (string.IsNullOrEmpty(userEmail))
        {
            return RedirectToAction("Login", "Login");
        }

        var result = _useService.ChangePassword(userEmail, model);

        if (result == "UserNotFound")
        {
            TempData["error"] = "User not found.";
            return View(model);
        }

        if (result == "IncorrectPassword")
        {
            TempData["error"] = "Current password is incorrect.";
            return View(model);
        }

        TempData["success"] = "Password updated successfully.";
        return RedirectToAction("Index", "Dashboard"); ;
    }

    public IActionResult Logout()
    {
        if (Request.Cookies["UserEmail"] != null)
        {
            Response.Cookies.Delete("UserEmail");
            Response.Cookies.Delete("AuthToken");

        }
        return RedirectToAction("Login", "Auth");
    }

}
