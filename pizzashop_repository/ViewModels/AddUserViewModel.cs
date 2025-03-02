using System.ComponentModel.DataAnnotations;
using pizzashop_repository.Models;

namespace pizzashop_repository.ViewModels
{
    public class AddUserViewModel
    {
        [Required]
        public string Firstname { get; set; }  = null!;

        [Required]
        public string Lastname { get; set; }  = null!;

        [Required]
        public string Username { get; set; }  = null!;

        [Required, EmailAddress]
        public string Email { get; set; }  = null!;

        [Required, MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }  = null!;

        [Required, Phone]
        public string Phone { get; set; }  = null!;

        [Required(ErrorMessage = "Please select a role.")]
        public int RoleId { get; set; }

        public string Country { get; set; }  = null!;
        public string State { get; set; }  = null!;
        public string City { get; set; }  = null!;

        [Required]
        public string Zipcode { get; set; }  = null!;

        [Required]
        public string Address { get; set; }  = null!;

        public string? Createdby { get; set; } 
    }
}
