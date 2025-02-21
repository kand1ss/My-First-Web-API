using System.ComponentModel.DataAnnotations;

namespace Application.DTO;

public record BookUpdateDTO(
    [MinLength(6)]
    string? Title,
    int? AuthorId,
    DateTime? PublishDate
    );