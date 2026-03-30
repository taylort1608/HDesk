using System;

namespace HDesk.Core.DTOs;

public record RegisterDto(
    string FullName,
    string Email,
    string Password
);

public record LoginDto(
    string Email,
    string Password
);
 
public record AuthResponseDto(
    string Token,
    string FullName,
    string Email,
    string Role
);
 