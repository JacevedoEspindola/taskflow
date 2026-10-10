using System;
using System.Collections.Generic;
using System.Text;


namespace TaskFlow.Application.Features.Columns.Dtos;

public record ColumnDetailDto(
    Guid Id,
    string Name,
    int Position,
    Guid ProjectId,
    int TaskCount,
    DateTime CreatedAt
);