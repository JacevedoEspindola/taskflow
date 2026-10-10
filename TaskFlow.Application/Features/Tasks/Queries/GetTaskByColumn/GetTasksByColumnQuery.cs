using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TaskFlow.Application.Features.Tasks.Dtos;

namespace TaskFlow.Application.Features.Tasks.Queries.GetTasksByColumn;

public record GetTasksByColumnQuery(Guid ColumnId) : IRequest<IEnumerable<TaskDto>>;
