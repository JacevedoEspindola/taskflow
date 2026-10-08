using MediatR;
using TaskFlow.Application.Features.Auth.Commands.Login;
using TaskFlow.Application.Features.Auth.Commands.Register;
using TaskFlow.Application.Features.Auth.Dtos;

namespace TaskFlow.API.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register", async (RegisterRequest request, IMediator mediator) =>
        {
            var command = new RegisterCommand(request.Email, request.Password, request.FullName);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("Register")
        .Produces<AuthResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/login", async (LoginRequest request, IMediator mediator) =>
        {
            var command = new LoginCommand(request.Email, request.Password);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("Login")
        .Produces<AuthResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        return app;
    }
}