namespace UserDirectoryAPI.Application.DTOs;

public record UserDto(
Guid Id,
string Name,
int Age,
string City,
string State,
string Pincode
);
