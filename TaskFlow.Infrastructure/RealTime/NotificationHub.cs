using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace TaskFlow.Infrastructure.RealTime;

[Authorize]
public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = this.Context.User?.FindFirst("sub")?.Value
                  ?? this.Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, $"user-{userId}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Los clientes llaman a este método para entrar a la sala de un proyecto.
    /// </summary>
    public async Task JoinProject(string projectId)
    {
        await this.Groups.AddToGroupAsync(this.Context.ConnectionId, $"project-{projectId}");
    }

    /// <summary>
    /// Los clientes llaman a este método para salir de la sala de un proyecto.
    /// </summary>
    public async Task LeaveProject(string projectId)
    {
        await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, $"project-{projectId}");
    }
}