using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HowManIsConnected.Models;

namespace HowManIsConnected.Services
{
    /// <summary>
    /// Tracks authenticated users in the Blazor Server application.
    /// </summary>
    public class UserTrackerService
    {
        private static readonly ConcurrentDictionary<string, UserInfo> _connectedUsers = new();
        public event Action<List<UserInfo>>? OnUserListChanged; // Notify UI when user list changes

        /// <summary>
        /// Adds a new user to the tracking list.
        /// </summary>
        public Task AddUser(string circuitId, string email, string name)
        {
            var userInfo = new UserInfo
            {
                CircuitId = circuitId,
                Email = email,
                Name = name
            };

            _connectedUsers[circuitId] = userInfo;
            NotifyClients();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Removes a user when their circuit is closed.
        /// </summary>
        public Task RemoveUser(string circuitId)
        {
            _connectedUsers.TryRemove(circuitId, out _);
            NotifyClients();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the current list of connected users.
        /// </summary>
        public List<UserInfo> GetConnectedUsers()
        {
            return _connectedUsers.Values.ToList();
        }

        private void NotifyClients()
        {
            var users = GetConnectedUsers();
            Console.WriteLine($"🔄 [UserTrackerService] Online users: {users.Count}");
            OnUserListChanged?.Invoke(users);
        }
    }
}
