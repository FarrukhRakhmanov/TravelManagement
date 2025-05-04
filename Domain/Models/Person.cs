using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    /// <summary>
    /// Person abstract class
    /// </summary>
    public abstract class Person
    {
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number is required.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        public string PhoneNumber { get; set; }


        //Returns the full name of the person
        public string FullName()
        {
            return $"{FirstName} {LastName}";
        }
    }
}
