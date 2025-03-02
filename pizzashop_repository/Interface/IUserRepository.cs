using pizzashop_repository.Models;
using pizzashop_repository.ViewModels;

namespace pizzashop_repository.Interface;

public interface IUserRepository
{
    User? GetUserByEmail(string email);
    bool UpdateUser(User user);

    string? GetUserRole(int roleId);

    User? GetUserByEmailAndRole(string email);
    
    IQueryable<User> GetUsersQuery();

    User GetUserById(int id);

    void SoftDeleteUser(User user);

    List<Role> GetRoles();
    Role GetRoleById(int id);

    void AddUser(User user);

    User? GetUserByIdAndRole(int id);

}
