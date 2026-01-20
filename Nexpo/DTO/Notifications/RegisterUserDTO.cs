
using System.ComponentModel.DataAnnotations;

public class RegisterUserDTO
{
    [Required]
    public string Token { get; set; }

    [Required]
    public string Topic { get; set; }
}
