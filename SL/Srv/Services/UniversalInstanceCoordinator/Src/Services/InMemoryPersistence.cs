using System.Collections.Concurrent;
using SoulboundLegends.InstanceCoordinator.Server.Abstractions;
using SoulboundLegends.InstanceCoordinator.Server.Models;
namespace SoulboundLegends.InstanceCoordinator.Server.Services;
public sealed class InMemoryPersistence : IInstancePersistence
{
 private readonly ConcurrentDictionary<string,InstanceRecord> records=new();
 private readonly ConcurrentQueue<string> audit=new();
 public Task SaveAsync(InstanceRecord r,CancellationToken ct){ records[r.InstanceId]=Clone(r); return Task.CompletedTask; }
 public Task<InstanceRecord?> LoadAsync(string id,CancellationToken ct){ records.TryGetValue(id,out var r); return Task.FromResult(r==null?null:Clone(r)); }
 public Task<IReadOnlyList<InstanceRecord>> LoadRecoverableAsync(CancellationToken ct)=>Task.FromResult<IReadOnlyList<InstanceRecord>>(records.Values.Where(x=>x.State is not LifecycleState.CleanupComplete and not LifecycleState.Destroyed).Select(Clone).ToList());
 public Task WriteAuditAsync(string action,string id,string actor,string detail,CancellationToken ct){audit.Enqueue($"{DateTimeOffset.UtcNow:o}|{action}|{id}|{actor}|{detail}");return Task.CompletedTask;}
 private static InstanceRecord Clone(InstanceRecord x)=>new(){InstanceId=x.InstanceId,InstanceType=x.InstanceType,WorldRegion=x.WorldRegion,ServerId=x.ServerId,MaximumCapacity=x.MaximumCapacity,ReservedSlots=x.ReservedSlots,PlayerIds=[..x.PlayerIds],PartyIds=[..x.PartyIds],State=x.State,FailureState=x.FailureState,FailureReason=x.FailureReason,GameplayMode=x.GameplayMode,Ruleset=x.Ruleset,MapEnvironment=x.MapEnvironment,CreatedAt=x.CreatedAt,LastActivityAt=x.LastActivityAt,ExpiresAt=x.ExpiresAt,PersistenceRequired=x.PersistenceRequired,PersistentWorldRelationship=x.PersistentWorldRelationship,Version=x.Version};
}
