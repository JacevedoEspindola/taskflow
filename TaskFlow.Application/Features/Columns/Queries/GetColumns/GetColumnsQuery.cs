using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TaskFlow.Application.Features.Columns.Dtos;

namespace TaskFlow.Application.Features.Columns.Queries.GetColumns;

public record GetColumnsQuery(Guid ProjectId) : IRequest<IEnumerable<ColumnDetailDto>>;