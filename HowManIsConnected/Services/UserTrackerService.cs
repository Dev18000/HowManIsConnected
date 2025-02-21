using System.Collections.Concurrent;
using HowManIsConnected.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace HowManIsConnected.Services
{
    public class UserTrackerService
    {
        private static readonly ConcurrentDictionary<string, UserInfo> _connectedUsers = new();
        public event Action<List<UserInfo>>? OnUserListChanged;
        private readonly IServiceProvider _serviceProvider;

        public UserTrackerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task AddUser(string circuitId, string email, string name)
        {
            using var scope = _serviceProvider.CreateScope();
            var authStateProvider = scope.ServiceProvider.GetRequiredService<AuthenticationStateProvider>();
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (string.IsNullOrWhiteSpace(email))
            {
                email = user.FindFirst(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value ?? "guest@example.com";
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                name = user.Identity?.Name ?? "Guest User";
            }

            var userInfo = new UserInfo
            {
                CircuitId = circuitId,
                Email = email,
                Name = name
            };

            _connectedUsers[circuitId] = userInfo;
            NotifyClients();
        }

        public Task RemoveUser(string circuitId)
        {
            _connectedUsers.TryRemove(circuitId, out _);
            NotifyClients();
            return Task.CompletedTask;
        }

        public List<UserInfo> GetConnectedUsers()
        {
            return _connectedUsers.Values.ToList();
        }

        public async Task HandleAuthStateChanged()
        {
            Console.WriteLine($"🔄 [UserTrackerService] Handling Auth State Change...");

            using var scope = _serviceProvider.CreateScope();
            var authStateProvider = scope.ServiceProvider.GetRequiredService<AuthenticationStateProvider>();
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            foreach (var entry in _connectedUsers)
            {
                entry.Value.Email = user.FindFirst(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value ?? "guest@example.com";
                entry.Value.Name = user.Identity?.Name ?? "Guest User";
            }

            NotifyClients();
        }

        private void NotifyClients()
        {
            var users = GetConnectedUsers();
            Console.WriteLine($"🔄 [UserTrackerService] Updating UI: {users.Count} users");
            OnUserListChanged?.Invoke(users);
        }
    }
}
