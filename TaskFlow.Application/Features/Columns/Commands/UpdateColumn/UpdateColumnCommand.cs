using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TaskFlow.Application.Features.Columns.Dtos;

namespace TaskFlow.Application.Features.Columns.Commands.UpdateColumn;

public record UpdateColumnCommand(
    Guid ProjectId,
    Guid ColumnId,
    string Name,
    int? Position
) : IRequest<ColumnDetailDto>;