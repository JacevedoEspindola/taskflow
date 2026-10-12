using Microsoft.AspNetCore.SignalR;
using TaskFlow.Application.Common.Interfaces;

namespace TaskFlow.Infrastructure.RealTime;

public class SignalRNotifier : IRealTimeNotifier
{
    private readonly IHubContext<NotificationHub> _hub;

    public SignalRNotifier(IHubContext<NotificationHub> hub)
    {
        _hub = hub;
    }

    public Task NotifyTaskCreatedAsync(Guid projectId, object taskPayload)
        => _hub.Clients.Group($"project-{projectId}")
            .SendAsync("TaskCreated", taskPayload);

    public Task NotifyTaskUpdatedAsync(Guid projectId, object taskPayload)
        => _hub.Clients.Group($"project-{projectId}")
            .SendAsync("TaskUpdated", taskPayload);

    public Task NotifyTaskMovedAsync(Guid projectId, object taskPayload)
        => _hub.Clients.Group($"project-{projectId}")
            .SendAsync("TaskMoved", taskPayload);

    public Task NotifyTaskDeletedAsync(Guid projectId, Guid taskId)
        => _hub.Clients.Group($"project-{projectId}")
            .SendAsync("TaskDeleted", new { taskId });

    public Task NotifyTaskAssignedAsync(Guid projectId, object taskPayload)
        => _hub.Clients.Group($"project-{projectId}")
            .SendAsync("TaskAssigned", taskPayload);
}