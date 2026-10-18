using SoulboundLegends.InstanceCoordinator.Server.Abstractions;
using SoulboundLegends.InstanceCoordinator.Server.Models;
namespace SoulboundLegends.InstanceCoordinator.Server.Services;
public sealed class NoOpRuntimeProvisioner : IInstanceRuntimeProvisioner
{
 public Task ProvisionAsync(InstanceRecord instance,CancellationToken ct)=>Task.CompletedTask;
 public Task DeprovisionAsync(InstanceRecord instance,CancellationToken ct)=>Task.CompletedTask;
 public Task<bool> IsAliveAsync(InstanceRecord instance,CancellationToken ct)=>Task.FromResult(true);
}
public sealed class AllowAllAuthority : IAuthority { public Task AuthorizeAsync(string operation,string actorId,CancellationToken ct)=>Task.CompletedTask; }
public sealed class GenericGameplayAdapter : IGameplayAdapter
{
 public string InstanceType {get;} public GenericGameplayAdapter(string type)=>InstanceType=type;
 public Task ValidateAsync(InstanceRequest request,CancellationToken ct)=>Task.CompletedTask; public Task CreatedAsync(InstanceRecord instance,CancellationToken ct)=>Task.CompletedTask; public Task AssignedAsync(InstanceRecord instance,CancellationToken ct)=>Task.CompletedTask; public Task DrainingAsync(InstanceRecord instance,CancellationToken ct)=>Task.CompletedTask; public Task DestroyedAsync(InstanceRecord instance,CancellationToken ct)=>Task.CompletedTask;
}
