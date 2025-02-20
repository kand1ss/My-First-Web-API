using System.ComponentModel.DataAnnotations;

namespace Application.DTO;

public record UpdateDTO(
    [MinLength(5)] 
    string? Login,
    [MinLength(8)] 
    string? Password,
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    string? Email,
    string? FirstName,
    string? LastName);
