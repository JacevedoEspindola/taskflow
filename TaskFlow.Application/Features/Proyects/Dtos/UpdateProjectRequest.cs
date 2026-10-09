using System;
using System.Collections.Generic;
using System.Text;
namespace TaskFlow.Application.Features.Projects.Dtos;

public record UpdateProjectRequest(
    string Name,
    string? Description
);