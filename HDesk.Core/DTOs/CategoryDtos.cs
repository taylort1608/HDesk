using System;

namespace HDesk.Core.DTOs;

public record CreateCategoryDto(string Name, string? Description);
 
public record UpdateCategoryDto(string? Name, string? Description, bool? IsActive);
 
public record CategoryDto(int Id, string Name, string? Description, bool IsActive);
 