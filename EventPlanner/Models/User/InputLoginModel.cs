using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models.User
{
    public class InputLoginModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
