namespace pizzashop_repository.ViewModels;

public class UserTableViewModel
{
    public string Email { get; set; } = null!;
    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string Username { get; set; } = null!;


    public string Phone { get; set; } = null!;

    public string? Rolename { get; set; }

    // public int CountryId { get; set; }
    // public int StateId { get; set; }
    // public int CityId { get; set; }
    public string? Country { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }

    public string Zipcode { get; set; } = null!;

    public string Address { get; set; } = null!;
}
