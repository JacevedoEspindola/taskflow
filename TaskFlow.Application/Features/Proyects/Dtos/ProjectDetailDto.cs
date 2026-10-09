using System;
using System.Collections.Generic;
using System.Text;


namespace TaskFlow.Application.Features.Projects.Dtos;

public record ProjectDetailDto(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    Guid OwnerId,
    IEnumerable<ColumnDto> Columns
);

public record ColumnDto(
    Guid Id,
    string Name,
    int Position,
    int TaskCount
);