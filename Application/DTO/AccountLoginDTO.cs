using System.ComponentModel.DataAnnotations;

namespace Application.DTO;

public record AccountLoginDTO(
    [Required]
    [MinLength(5)] 
    string Login, 
    [Required]
    [MinLength(8)] 
    string Password);