
using pizzashop_repository.Models;
using pizzashop_repository.ViewModels;

namespace pizzashop_service.Interface;

public interface IUserService
{
    User? AuthenicateUser(string email, string password);
    string GenerateJwttoken(string email, int roleid);

    bool ResetPassword(string email, string newPassword, string confirmPassword, out string message);

    string ExtractEmailFromToken(string token);
    UserTableViewModel? GetUserProfile(string email);

    bool UpdateUserProfile(string email, UserTableViewModel model);

    string? ChangePassword(string email, ChangePasswordViewModel model);
    
    PaginatedUserViewModel GetUsers(string searchTerm, int page, int pageSize, string sortBy, string sortOrder);

    bool DeleteUser(int id);

    List<Role> GetRoles();
    
    bool AddUser(AddUserViewModel model);

    EditUserViewModel GetUserForEdit(int id);

    bool EditUser(int id, EditUserViewModel model);
    
}
