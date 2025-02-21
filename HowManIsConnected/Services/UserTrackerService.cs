using System.Collections.Concurrent;
using System.Security.Claims;
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

            foreach (var (circuitId, user) in _connectedUsers)
            {
                using var scope = _serviceProvider.CreateScope();
                var authStateProvider = scope.ServiceProvider.GetRequiredService<AuthenticationStateProvider>();
                var authState = await authStateProvider.GetAuthenticationStateAsync();
                var newUser = authState.User;

                if (newUser.Identity is { IsAuthenticated: true })
                {
                    user.Name = newUser.Identity?.Name ?? "Guest User";
                    user.Email = newUser.FindFirst(c => c.Type == ClaimTypes.Email)?.Value ?? "guest@example.com";
                }
            }

            NotifyClients();
        }

        private void NotifyClients()
        {
            var users = GetConnectedUsers();
            Console.WriteLine($"🔄 [UserTrackerService] Notifying Blazor: {users.Count} users");
            OnUserListChanged?.Invoke(new List<UserInfo>(users));
        }
    }
}
