using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using pizzashop_repository.Database;
using pizzashop_repository.Interface;
using pizzashop_repository.Models;
using pizzashop_repository.ViewModels;
using pizzashop_service.Interface;

namespace pizzashop_service.Implementation;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    private readonly PizzaShopDbContext _context;
    public UserService(IUserRepository userRepository, IJwtService jwtService , PizzaShopDbContext context)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _context = context;
       
    }

    public User? AuthenicateUser(string email, string password)
    {
        User? user = _userRepository.GetUserByEmail(email);

        if (user == null)
        {
            return null;
        }
        if (!user.Password.StartsWith("$2a$") && !user.Password.StartsWith("$2b$") && !user.Password.StartsWith("$2y$"))
        {
            // 🔹 First-time login: Hash and update the database
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.Password = hashedPassword;
            _userRepository.UpdateUser(user);
        }

        if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            return null;
        }
        return user;
    }

    public string GenerateJwttoken(string email, int roleId)
    {
        return _jwtService.GenerateJwtToken(email, roleId);
    }

    public bool ResetPassword(string email, string newPassword, string confirmPassword, out string message)
    {
        if (newPassword != confirmPassword)
        {
            message = "The new Password and Confirmation password do not match";
            return false;
        }
        var user = _userRepository.GetUserByEmail(email);
        if (user == null)
        {
            message = "User not found";
            return false;
        }

        user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
        _userRepository.UpdateUser(user);
        message = "Password has been successfully updated";
        return true;
    }

    public string ExtractEmailFromToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return string.Empty;

        var handler = new JwtSecurityTokenHandler();
        var AuthToken = handler.ReadJwtToken(token);
        return AuthToken.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Email)?.Value ?? string.Empty;
    }

    public UserTableViewModel? GetUserProfile(string email)
    {
        if (string.IsNullOrEmpty(email))
            return null;

        var user = _userRepository.GetUserByEmailAndRole(email);
        if (user == null)
            return null;

        return new UserTableViewModel
        {
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Username = user.Username,
            Email = user.Email,
            Rolename = user.Role.Rolename,
            Country = user.Country,
            State = user.State,
            City = user.City,
            Phone = user.Phone,
            Address = user.Address,
            Zipcode = user.Zipcode
        };
    }

    public bool UpdateUserProfile(string email, UserTableViewModel model)
    {
        var user = _userRepository.GetUserByEmail(email);
        var countryname = _context.Countries.FirstOrDefault(c => c.Id.ToString() == model.Country);
        var statename = _context.States.FirstOrDefault(c => c.Id.ToString() == model.State);
        var cityname = _context.Cities.FirstOrDefault(c => c.Id.ToString() == model.City);

        model.Country = countryname?.Countryname;
        model.State = statename?.Statename;
        model.City = cityname?.Cityname;
        if (user == null) return false;

        user.Firstname = model.Firstname;
        user.Lastname = model.Lastname;
        user.Username = model.Username;
        user.Phone = model.Phone;
        user.Country = model.Country;
        user.State = model.State;
        user.City = model.City;
        user.Address = model.Address;
        user.Zipcode = model.Zipcode;

        return _userRepository.UpdateUser(user);
    }

    public string ChangePassword(string email, ChangePasswordViewModel model)
    {
        var user = _userRepository.GetUserByEmail(email);
        
        if (user == null)
        {
            return "UserNotFound";
        }

        if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.Password))
        {
            return "IncorrectPassword";
        }

        user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
        _userRepository.UpdateUser(user);

        return "Success";
    }

    public PaginatedUserViewModel GetUsers(string searchTerm, int page, int pageSize, string sortBy, string sortOrder)
    {
        var query = _userRepository.GetUsersQuery();

        // Apply search filter
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(u => u.Firstname.Contains(searchTerm) || u.Lastname.Contains(searchTerm));
        }

        // Sorting logic
        query = sortBy switch
        {
            "Name" => sortOrder == "asc"
                ? query.OrderBy(u => u.Firstname).ThenBy(u => u.Lastname)
                : query.OrderByDescending(u => u.Firstname).ThenByDescending(u => u.Lastname),

            "Role" => sortOrder == "asc"
                ? query.OrderBy(u => u.Role.Rolename)
                : query.OrderByDescending(u => u.Role.Rolename),

            _ => query.OrderBy(u => u.Id) // Default sorting by ID
        };

        int totalItems = query.Count();
        var users = query.Skip((page - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();

        return new PaginatedUserViewModel
        {
            Users = users,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            SortBy = sortBy,
            SortOrder = sortOrder
        };
    }

    public bool DeleteUser(int id)
    {
        var user = _userRepository.GetUserById(id);
        if (user == null)
        {
            return false; // User not found
        }

        _userRepository.SoftDeleteUser(user);
        return true;
    }

     public List<Role> GetRoles()
    {
        return _userRepository.GetRoles();
    }

    public bool AddUser(AddUserViewModel model)
    {
        var role = _userRepository.GetRoleById(model.RoleId);
        if (role == null)
        {
            return false; // Role does not exist
        }

        var countryname = _context.Countries.FirstOrDefault(c => c.Id.ToString() == model.Country);
        var statename = _context.States.FirstOrDefault(c => c.Id.ToString() == model.State);
        var cityname = _context.Cities.FirstOrDefault(c => c.Id.ToString() == model.City);

        model.Country = countryname?.Countryname;
        model.State = statename?.Statename;
        model.City = cityname?.Cityname;

        var user = new User
        {
            Firstname = model.Firstname,
            Lastname = model.Lastname,
            Email = model.Email,
            Username = model.Username,
            Phone = model.Phone,
            Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Roleid = model.RoleId,
            Country = model.Country,
            State = model.State,
            City = model.City,
            Address = model.Address,
            Zipcode = model.Zipcode,
            Createdby = role.Rolename
        };

        _userRepository.AddUser(user);

        // Send email with login details
        // string filePath = @"Y:/tryerror-main/Template/AddNewUserEmailTemplate.html";
        // string emailBody = File.ReadAllText(filePath);
        // emailBody = emailBody.Replace("{Email}", model.Email);
        // emailBody = emailBody.Replace("{Password}", model.Password);

        // string subject = "Your Login Details";
        // _emailSender.SendEmailAsync(model.Email, subject, emailBody);

        return true;
    }

    public EditUserViewModel GetUserForEdit(int id)
    {
        var user = _userRepository.GetUserById(id);

        if (user == null) return null;

        return new EditUserViewModel
        {
            id = user.Id,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Email = user.Email,
            Username = user.Username,
            Phone = user.Phone,
            Status = user.Status,
            RoleId = user.Roleid,
            Country = user.Country,
            State = user.State,
            City = user.City,
            Address = user.Address,
            Zipcode = user.Zipcode
        };
    }

    public bool EditUser(int id, EditUserViewModel model)
    {
        var user = _userRepository.GetUserById(id);
        if (user == null) return false;

        var countryname = _context.Countries.FirstOrDefault(c => c.Id.ToString() == model.Country);
        var statename = _context.States.FirstOrDefault(c => c.Id.ToString() == model.State);
        var cityname = _context.Cities.FirstOrDefault(c => c.Id.ToString() == model.City);

        model.Country = countryname?.Countryname;
        model.State = statename?.Statename;
        model.City = cityname?.Cityname;


        user.Firstname = model.Firstname;
        user.Lastname = model.Lastname;
        user.Email = model.Email;
        user.Username = model.Username;
        user.Phone = model.Phone;
        user.Status = model.Status;
        user.Roleid = model.RoleId;
        user.Country = model.Country;
        user.State = model.State;
        user.City = model.City;
        user.Address = model.Address;
        user.Zipcode = model.Zipcode;

        _userRepository.UpdateUser(user);
        return true;
    }
}
