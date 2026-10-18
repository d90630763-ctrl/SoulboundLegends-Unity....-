using System.Collections.Concurrent;
using SoulboundLegends.InstanceCoordinator.Server.Models;
namespace SoulboundLegends.InstanceCoordinator.Server.Services;
public sealed class CapacityReservationManager
{
 private readonly ConcurrentDictionary<string,Reservation> reservations=new();
 public bool TryReserve(InstanceRecord instance,string requestId,IReadOnlyCollection<string> players,TimeSpan ttl,out Reservation reservation){reservation=new(){ReservationId=$"res_{Guid.NewGuid():N}",InstanceId=instance.InstanceId,RequestId=requestId,Slots=players.Count,PlayerIds=[..players],CreatedAt=DateTimeOffset.UtcNow,ExpiresAt=DateTimeOffset.UtcNow+ttl};lock(instance){if(instance.State!=LifecycleState.Running||instance.MaximumCapacity-(instance.PlayerIds.Count+instance.ReservedSlots)<reservation.Slots)return false;instance.ReservedSlots+=reservation.Slots;}reservations[reservation.ReservationId]=reservation;return true;}
 public bool Commit(string id){if(!reservations.TryGetValue(id,out var r)||r.ExpiresAt<DateTimeOffset.UtcNow)return false;r.Committed=true;return true;}
 public bool Release(string id,InstanceRecord instance){if(!reservations.TryRemove(id,out var r))return false;lock(instance)instance.ReservedSlots=Math.Max(0,instance.ReservedSlots-r.Slots);return true;}
 public int ExpireAll(Func<string,InstanceRecord?> lookup){var n=0;foreach(var kv in reservations.ToArray())if(kv.Value.ExpiresAt<DateTimeOffset.UtcNow){if(lookup(kv.Value.InstanceId) is { } i)Release(kv.Key,i);else reservations.TryRemove(kv.Key,out _);n++;}return n;}
}
