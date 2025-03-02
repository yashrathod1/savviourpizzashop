using System;
using System.Collections.Generic;

namespace pizzashop_repository.Models;

public partial class User
{
    public int Id { get; set; }

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string State { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Zipcode { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? Status { get; set; }

    public bool Isdeleted { get; set; }

    public int Roleid { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updateat { get; set; }

    public string Createdby { get; set; } = null!;

    public string? Updatedby { get; set; }

    public virtual Role Role { get; set; } = null!;
}
