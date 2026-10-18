using System.Net.Http.Json;
using SoulboundLegends.InstanceCoordinator.Server.Abstractions;
using SoulboundLegends.InstanceCoordinator.Server.Models;
namespace SoulboundLegends.InstanceCoordinator.Server.Services;
public sealed class HttpRuntimeProvisioner : IInstanceRuntimeProvisioner
{
 private readonly HttpClient http; private readonly string baseUrl;
 public HttpRuntimeProvisioner(HttpClient http,string baseUrl){this.http=http;this.baseUrl=baseUrl.TrimEnd('/');}
 public async Task ProvisionAsync(InstanceRecord instance,CancellationToken ct){var r=await http.PostAsJsonAsync($"{baseUrl}/instances/provision",instance,ct);r.EnsureSuccessStatusCode();}
 public async Task DeprovisionAsync(InstanceRecord instance,CancellationToken ct){var r=await http.PostAsJsonAsync($"{baseUrl}/instances/deprovision",instance,ct);r.EnsureSuccessStatusCode();}
 public async Task<bool> IsAliveAsync(InstanceRecord instance,CancellationToken ct){var r=await http.GetAsync($"{baseUrl}/instances/{Uri.EscapeDataString(instance.InstanceId)}/health",ct);return r.IsSuccessStatusCode;}
}
