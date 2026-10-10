using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TaskFlow.Application.Features.Columns.Dtos;

namespace TaskFlow.Application.Features.Columns.Commands.CreateColumn;

public record CreateColumnCommand(
    Guid ProjectId,
    string Name
) : IRequest<ColumnDetailDto>;