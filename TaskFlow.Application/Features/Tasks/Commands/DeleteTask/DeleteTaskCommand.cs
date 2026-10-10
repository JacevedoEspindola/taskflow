using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace TaskFlow.Application.Features.Tasks.Commands.DeleteTask;

public record DeleteTaskCommand(Guid TaskId) : IRequest<Unit>;