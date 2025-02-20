using System.ComponentModel.DataAnnotations;

namespace Application.DTO;

public record RegisterDTO(
    [Required]
    [MinLength(5)] 
    string Login, 
    [Required]
    [MinLength(8)] 
    string Password,
    [Required] 
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    string Email, 
    string? FirstName, 
    string? LastName);