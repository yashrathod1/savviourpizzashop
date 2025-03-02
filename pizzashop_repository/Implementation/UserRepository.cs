using Microsoft.EntityFrameworkCore;
using pizzashop_repository.Database;
using pizzashop_repository.Interface;
using pizzashop_repository.Models;
using pizzashop_repository.ViewModels;

namespace pizzashop_repository.Implementation;

public class UserRepository : IUserRepository
{
    private readonly PizzaShopDbContext _context;

    public UserRepository(PizzaShopDbContext context)
    {
        _context = context;
    }

    public User? GetUserByEmail(string email)
    {
        return _context.Users.FirstOrDefault(u => u.Email == email);
    }

    public bool UpdateUser(User user)
    {
        _context.Users.Update(user);
        _context.SaveChanges();
        return true;
    }

    public string? GetUserRole(int roleId)
    {
        return _context.Roles.Where(r => r.Id == roleId).Select(r => r.Rolename).FirstOrDefault();
    }

    public User? GetUserByEmailAndRole(string email)
    {
         return _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == email);
    }

    public IQueryable<User> GetUsersQuery()
    {
        return _context.Users.Include(u => u.Role).Where(u => !u.Isdeleted);
    }

    public User? GetUserById(int id)
    {
        return _context.Users.FirstOrDefault(u => u.Id == id);
    }

    public void SoftDeleteUser(User user)
    {
        user.Isdeleted = true;
        _context.SaveChanges();
    }
    
    public List<Role> GetRoles()
    {
        return _context.Roles.ToList();
    }

    public Role GetRoleById(int id)
    {
        return _context.Roles.FirstOrDefault(r => r.Id == id);
    }

     public void AddUser(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
    }


}
