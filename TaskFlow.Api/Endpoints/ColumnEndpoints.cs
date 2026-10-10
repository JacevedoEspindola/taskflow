using MediatR;
using TaskFlow.Application.Features.Columns.Commands.CreateColumn;
using TaskFlow.Application.Features.Columns.Commands.DeleteColumn;
using TaskFlow.Application.Features.Columns.Commands.UpdateColumn;
using TaskFlow.Application.Features.Columns.Dtos;
using TaskFlow.Application.Features.Columns.Queries.GetColumns;

namespace TaskFlow.API.Endpoints;

public static class ColumnEndpoints
{
    public static IEndpointRouteBuilder MapColumnEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/projects/{projectId:guid}/columns")
            .WithTags("Columns")
            .RequireAuthorization();

        // GET /api/projects/{projectId}/columns
        group.MapGet("/", async (Guid projectId, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetColumnsQuery(projectId));
            return Results.Ok(result);
        })
        .WithName("GetColumns")
        .Produces<IEnumerable<ColumnDetailDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        // POST /api/projects/{projectId}/columns
        group.MapPost("/", async (Guid projectId, CreateColumnRequest request, IMediator mediator) =>
        {
            var command = new CreateColumnCommand(projectId, request.Name);
            var result = await mediator.Send(command);
            return Results.Created(
                $"/api/projects/{projectId}/columns/{result.Id}",
                result);
        })
        .WithName("CreateColumn")
        .Produces<ColumnDetailDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized);

        // PUT /api/projects/{projectId}/columns/{columnId}
        group.MapPut("/{columnId:guid}", async (
            Guid projectId,
            Guid columnId,
            UpdateColumnRequest request,
            IMediator mediator) =>
        {
            var command = new UpdateColumnCommand(projectId, columnId, request.Name, request.Position);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("UpdateColumn")
        .Produces<ColumnDetailDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status401Unauthorized);

        // DELETE /api/projects/{projectId}/columns/{columnId}
        group.MapDelete("/{columnId:guid}", async (Guid projectId, Guid columnId, IMediator mediator) =>
        {
            await mediator.Send(new DeleteColumnCommand(projectId, columnId));
            return Results.NoContent();
        })
        .WithName("DeleteColumn")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status401Unauthorized);

        return app;
    }
}