namespace UserService.Api.Dtos;

public record RegisterUserDto(string Username, string Email, string Password);
public record LoginUserDto(string Username, string Password);
