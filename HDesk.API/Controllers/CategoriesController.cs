using System;
using HDesk.Core.DTOs;
using HDesk.Core.Interfaces;
using HDesk.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ITicketCategoryRepository categories) : ControllerBase
{
    // Get ticket categories
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
    {
        // Allow admins to see inactive categories
        if (includeInactive && !(User.IsInRole("Admin")))
            return Forbid();

        var list = await categories.GetAllAsync(includeInactive);

        return Ok(list.Select(ToDto));
    }

    // Get category by ID
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await categories.GetByIdAsync(id);
        return category is null ? NotFound() : Ok(ToDto(category));
    }

    // Admin only - Create category
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateCategoryDto createCategoryDto)
    {
        var category = new TicketCategory
        {
            Name = createCategoryDto.Name,
            Description = createCategoryDto.Description
        };

        var created = await categories.CreateAsync(category);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
    }

    // Admin only - Update category
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, UpdateCategoryDto updateCategoryDto)
    {
        var category = await categories.GetByIdAsync(id);
        if (category is null) return NotFound();
        if (updateCategoryDto.Name is not null) category.Name = updateCategoryDto.Name;
        if (updateCategoryDto.Description is not null) category.Description = updateCategoryDto.Description;
        if (updateCategoryDto.IsActive is not null) category.IsActive = updateCategoryDto.IsActive.Value;

        var updated = await categories.UpdateAsync(category);
        
        return Ok(ToDto(updated));
    }

    private static CategoryDto ToDto(TicketCategory c) =>
        new(c.Id, c.Name, c.Description, c.IsActive);
}
