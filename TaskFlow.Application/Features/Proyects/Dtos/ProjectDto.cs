using System;
using System.Collections.Generic;
using System.Text;
namespace TaskFlow.Application.Features.Projects.Dtos;

public record ProjectDto(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    int MemberCount,
    int ColumnCount
);