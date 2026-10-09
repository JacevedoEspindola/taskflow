using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Application.Features.Projects.Dtos;

public record CreateProjectRequest(
    string Name,
    string? Description
);