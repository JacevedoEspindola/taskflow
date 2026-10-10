using MediatR;
using TaskFlow.Application.Features.Tasks.Commands.AssignTask;
using TaskFlow.Application.Features.Tasks.Commands.CreateTask;
using TaskFlow.Application.Features.Tasks.Commands.DeleteTask;
using TaskFlow.Application.Features.Tasks.Commands.MoveTask;
using TaskFlow.Application.Features.Tasks.Commands.UpdateTask;
using TaskFlow.Application.Features.Tasks.Dtos;
using TaskFlow.Application.Features.Tasks.Queries.GetTaskById;
using TaskFlow.Application.Features.Tasks.Queries.GetTasksByColumn;

namespace TaskFlow.API.Endpoints;

public static class TaskEndpoints
{
    public static IEndpointRouteBuilder MapTaskEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api")
            .WithTags("Tasks")
            .RequireAuthorization();

        // ============================================
        // GET /api/columns/{columnId}/tasks
        // ============================================
        group.MapGet("/columns/{columnId:guid}/tasks", async (Guid columnId, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetTasksByColumnQuery(columnId));
            return Results.Ok(result);
        })
        .WithName("GetTasksByColumn")
        .Produces<IEnumerable<TaskDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        // ============================================
        // GET /api/tasks/{id}
        // ============================================
        group.MapGet("/tasks/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetTaskByIdQuery(id));
            return Results.Ok(result);
        })
        .WithName("GetTaskById")
        .Produces<TaskDetailDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status401Unauthorized);

        // ============================================
        // POST /api/columns/{columnId}/tasks
        // ============================================
        group.MapPost("/columns/{columnId:guid}/tasks", async (
            Guid columnId,
            CreateTaskRequest request,
            IMediator mediator) =>
        {
            var command = new CreateTaskCommand(
                columnId,
                request.Title,
                request.Description,
                request.Priority,
                request.DueDate);

            var result = await mediator.Send(command);
            return Results.Created($"/api/tasks/{result.Id}", result);
        })
        .WithName("CreateTask")
        .Produces<TaskDetailDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized);

        // ============================================
        // PUT /api/tasks/{id}
        // ============================================
        group.MapPut("/tasks/{id:guid}", async (
            Guid id,
            UpdateTaskRequest request,
            IMediator mediator) =>
        {
            var command = new UpdateTaskCommand(
                id,
                request.Title,
                request.Description,
                request.Priority,
                request.DueDate);

            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("UpdateTask")
        .Produces<TaskDetailDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status401Unauthorized);

        // ============================================
        // PATCH /api/tasks/{id}/move
        // ============================================
        group.MapPatch("/tasks/{id:guid}/move", async (
            Guid id,
            MoveTaskRequest request,
            IMediator mediator) =>
        {
            var command = new MoveTaskCommand(id, request.TargetColumnId, request.Position);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("MoveTask")
        .Produces<TaskDetailDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status401Unauthorized);

        // ============================================
        // PATCH /api/tasks/{id}/assign
        // ============================================
        group.MapPatch("/tasks/{id:guid}/assign", async (
            Guid id,
            AssignTaskRequest request,
            IMediator mediator) =>
        {
            var command = new AssignTaskCommand(id, request.AssigneeId);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("AssignTask")
        .Produces<TaskDetailDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status401Unauthorized);

        // ============================================
        // DELETE /api/tasks/{id}
        // ============================================
        group.MapDelete("/tasks/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            await mediator.Send(new DeleteTaskCommand(id));
            return Results.NoContent();
        })
        .WithName("DeleteTask")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status401Unauthorized);

        return app;
    }
}