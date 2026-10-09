using MediatR;
using Microsoft.AspNetCore.Authorization;
using TaskFlow.Application.Features.Projects.Commands.CreateProject;
using TaskFlow.Application.Features.Projects.Commands.DeleteProject;
using TaskFlow.Application.Features.Projects.Commands.UpdateProject;
using TaskFlow.Application.Features.Projects.Dtos;
using TaskFlow.Application.Features.Projects.Queries.GetProjectById;
using TaskFlow.Application.Features.Projects.Queries.GetProjects;

namespace TaskFlow.API.Endpoints;

public static class ProjectEndpoints
{
    public static IEndpointRouteBuilder MapProjectEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/projects")
            .WithTags("Projects")
            .RequireAuthorization(); // ⚠️ Todo el grupo requiere JWT

        // GET /api/projects
        group.MapGet("/", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetProjectsQuery());
            return Results.Ok(result);
        })
        .WithName("GetProjects")
        .Produces<IEnumerable<ProjectDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        // GET /api/projects/{id}
        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetProjectByIdQuery(id));
            return Results.Ok(result);
        })
        .WithName("GetProjectById")
        .Produces<ProjectDetailDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status401Unauthorized);

        // POST /api/projects
        group.MapPost("/", async (CreateProjectRequest request, IMediator mediator) =>
        {
            var command = new CreateProjectCommand(request.Name, request.Description);
            var result = await mediator.Send(command);
            return Results.Created($"/api/projects/{result.Id}", result);
        })
        .WithName("CreateProject")
        .Produces<ProjectDetailDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized);

        // PUT /api/projects/{id}
        group.MapPut("/{id:guid}", async (Guid id, UpdateProjectRequest request, IMediator mediator) =>
        {
            var command = new UpdateProjectCommand(id, request.Name, request.Description);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("UpdateProject")
        .Produces<ProjectDetailDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status401Unauthorized);

        // DELETE /api/projects/{id}
        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            await mediator.Send(new DeleteProjectCommand(id));
            return Results.NoContent();
        })
        .WithName("DeleteProject")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status401Unauthorized);

        return app;
    }
}