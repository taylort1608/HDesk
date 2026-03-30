using System;

namespace HDesk.Core.DTOs;

public record CreateTicketDto(
    string Title,
    string Description,
    int CategoryId,
    string Priority
);
 
public record UpdateTicketDto(
    string? Title,
    string? Description,
    int? CategoryId,
    string? Priority,
    string? Status
);
 
public record AssignTicketDto(int? AgentUserId); // null = unassign
 
public record TicketSummaryDto(
    int Id,
    string Title,
    string Status,
    string Priority,
    string Category,
    string SubmittedBy,
    string? AssignedTo,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
 
public record TicketDetailDto(
    int Id,
    string Title,
    string Description,
    string Status,
    string Priority,
    int CategoryId,
    string Category,
    int SubmittedByUserId,
    string SubmittedBy,
    int? AssignedToUserId,
    string? AssignedTo,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? ResolvedAt,
    IEnumerable<CommentDto> Comments
);
 
public record CommentDto(
    int Id,
    string Body,
    bool IsInternal,
    string Author,
    DateTime CreatedAt
);
 
public record AddCommentDto(
    string Body,
    bool IsInternal
);