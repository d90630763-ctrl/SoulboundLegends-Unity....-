using System.Text.Json;
using SoulboundLegends.InstanceCoordinator.Server.Abstractions;
using SoulboundLegends.InstanceCoordinator.Server.Models;
namespace SoulboundLegends.InstanceCoordinator.Server.Services;
public sealed class JsonFilePersistence : IInstancePersistence
{
 private readonly string dir; private readonly SemaphoreSlim gate=new(1,1); private readonly JsonSerializerOptions options=new(JsonSerializerDefaults.Web){WriteIndented=false};
 public JsonFilePersistence(string directory){dir=directory;Directory.CreateDirectory(dir);}
 private string PathFor(string id)=>System.IO.Path.Combine(dir,$"{id}.json");
 public async Task SaveAsync(InstanceRecord r,CancellationToken ct){await gate.WaitAsync(ct);try{var tmp=PathFor(r.InstanceId)+".tmp";await File.WriteAllTextAsync(tmp,JsonSerializer.Serialize(r,options),ct);File.Move(tmp,PathFor(r.InstanceId),true);}finally{gate.Release();}}
 public async Task<InstanceRecord?> LoadAsync(string id,CancellationToken ct){var p=PathFor(id);if(!File.Exists(p))return null;return JsonSerializer.Deserialize<InstanceRecord>(await File.ReadAllTextAsync(p,ct),options);}
 public async Task<IReadOnlyList<InstanceRecord>> LoadRecoverableAsync(CancellationToken ct){var list=new List<InstanceRecord>();foreach(var p in Directory.EnumerateFiles(dir,"*.json")){ct.ThrowIfCancellationRequested();try{var r=JsonSerializer.Deserialize<InstanceRecord>(await File.ReadAllTextAsync(p,ct),options);if(r!=null&&r.State is not LifecycleState.CleanupComplete and not LifecycleState.Destroyed)list.Add(r);}catch{} }return list;}
 public Task WriteAuditAsync(string action,string id,string actor,string detail,CancellationToken ct)=>File.AppendAllTextAsync(System.IO.Path.Combine(dir,"audit.log"),$"{DateTimeOffset.UtcNow:o}|{action}|{id}|{actor}|{detail}{Environment.NewLine}",ct);
}
