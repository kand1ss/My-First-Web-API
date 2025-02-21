using System.ComponentModel.DataAnnotations;

namespace Application.DTO;

public record BookDTO(
    [Required]
    [MinLength(6)]
    string Title,
    [Required]
    int authorId,
    DateTime PublishDate
    );