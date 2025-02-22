using System.ComponentModel.DataAnnotations;

namespace Application.DTO;

public record AccountDTO(
    [Required]
    [MinLength(5)] 
    string Login, 
    [Required] 
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    string Email, 
    string? FirstName, 
    string? LastName);
