using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TaskFlow.Application.Features.Projects.Dtos;

namespace TaskFlow.Application.Features.Projects.Commands.CreateProject;

public record CreateProjectCommand(
    string Name,
    string? Description
) : IRequest<ProjectDetailDto>;