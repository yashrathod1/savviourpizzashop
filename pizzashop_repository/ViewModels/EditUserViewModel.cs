using System.ComponentModel.DataAnnotations;

namespace pizzashop_repository.ViewModels;

public class EditUserViewModel
{
    [Required]
        public string Firstname { get; set; } = null!;


        public int id { get; set; }

        [Required]
        public string Lastname { get; set; }= null!;

        [Required]
        public string Username { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Phone { get; set; } = null!;

    
        public int RoleId { get; set; }

   
        public string Country { get; set; } = null!;

    
        public string State { get; set; }= null!;

        public string City { get; set; } = null!;

        [Required]
        public string Zipcode { get; set; } = null!;

        [Required]
        public string Address { get; set; } = null!;

        public string Status { get; set; } = null!;
}
