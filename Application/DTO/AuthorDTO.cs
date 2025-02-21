using System.ComponentModel.DataAnnotations;

namespace Application.DTO;

public record AuthorDTO(
    [Required]
    [MinLength(4)]
    string FirstName,
    [Required]
    [MinLength(4)]
    string LastName
    );