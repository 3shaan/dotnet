using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

public record CreateGameDto(
    [Required][StringLength(50)] string Name,
    [MinLength(2)][StringLength(20)] string Genre,
    [Range(1, 100)] decimal Price,
    DateOnly Date
);