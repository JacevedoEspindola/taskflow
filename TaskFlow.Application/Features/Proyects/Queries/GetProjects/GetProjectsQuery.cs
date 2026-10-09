using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TaskFlow.Application.Features.Projects.Dtos;

namespace TaskFlow.Application.Features.Projects.Queries.GetProjects;

public record GetProjectsQuery() : IRequest<IEnumerable<ProjectDto>>;