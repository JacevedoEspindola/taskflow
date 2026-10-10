using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace TaskFlow.Application.Features.Columns.Commands.DeleteColumn;

public record DeleteColumnCommand(Guid ProjectId, Guid ColumnId) : IRequest<Unit>;
