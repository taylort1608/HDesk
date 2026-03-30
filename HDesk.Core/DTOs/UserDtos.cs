using System;

namespace HDesk.Core.DTOs;

public record UserSummaryDto(
    int Id,
    string FullName,
    string Email,
    string Role,
    bool IsActive,
    DateTime CreatedAt
);
 
public record UpdateUserRoleDto(string Role);
 
public record UpdateUserDto(string FullName);
 