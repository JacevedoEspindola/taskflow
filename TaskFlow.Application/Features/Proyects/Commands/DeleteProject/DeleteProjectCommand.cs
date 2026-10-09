using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace TaskFlow.Application.Features.Projects.Commands.DeleteProject;

public record DeleteProjectCommand(Guid ProjectId) : IRequest<Unit>;