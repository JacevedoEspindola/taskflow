using System;
using System.Collections.Generic;
using System.Text;
namespace TaskFlow.Application.Common.Interfaces;

public interface IRealTimeNotifier
{
    Task NotifyTaskCreatedAsync(Guid projectId, object taskPayload);
    Task NotifyTaskUpdatedAsync(Guid projectId, object taskPayload);
    Task NotifyTaskMovedAsync(Guid projectId, object taskPayload);
    Task NotifyTaskDeletedAsync(Guid projectId, Guid taskId);
    Task NotifyTaskAssignedAsync(Guid projectId, object taskPayload);
}