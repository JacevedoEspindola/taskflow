using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TaskFlow.Application.Features.Projects.Dtos;

namespace TaskFlow.Application.Features.Projects.Queries.GetProjectById;

public record GetProjectByIdQuery(Guid ProjectId) : IRequest<ProjectDetailDto>;