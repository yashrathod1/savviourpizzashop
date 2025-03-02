using Microsoft.AspNetCore.Mvc;
using pizzashop_repository.Models;
using pizzashop_repository.ViewModels;
using pizzashop_service.Interface;

namespace pizzashop.Controllers;

public class UsersController : Controller
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    public IActionResult UsersList(string searchTerm = "", int page = 1, int pageSize = 5, string sortBy = "Name", string sortOrder = "asc")
    {
        var paginatedUsers = _userService.GetUsers(searchTerm, page, pageSize, sortBy, sortOrder);

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return PartialView("_UsersList", paginatedUsers);
        }

        return View(paginatedUsers);
    }

    [HttpGet("Delete/{id}")]
    public IActionResult DeleteUser(int id)
    {
        bool isDeleted = _userService.DeleteUser(id);
        if (!isDeleted)
        {
            return NotFound();
        }

        return RedirectToAction("UsersList");
    }

    [HttpGet("AddUser")]
    public IActionResult AddUser()
    {
        ViewBag.Roles = _userService.GetRoles();
        return View();
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser( AddUserViewModel model)
    {
        ViewBag.Roles = _userService.GetRoles();

        if (ModelState.IsValid)
        {
            bool isAdded = _userService.AddUser(model);
            if (isAdded)
            {
                return RedirectToAction("UsersList", "Users");
            }
            ModelState.AddModelError("", "Failed to add user. Role may not exist.");
        }

        return View(model);
    }
}
