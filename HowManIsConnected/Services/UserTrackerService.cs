using System.Collections.Concurrent;

namespace HowManIsConnected.Services
{
    /// <summary>
    /// Tracks the number of connected users in a Blazor Server application.
    /// </summary>
    public class UserTrackerService
    {
        // Stores connected users by Circuit ID
        private static readonly ConcurrentDictionary<string, bool> _connectedCircuits = new();

        /// <summary>
        /// Event triggered when the user count changes.
        /// </summary>
        public event Action<int>? OnUserCountChanged;

        /// <summary>
        /// Adds a user when a new circuit (connection) is opened.
        /// </summary>
        public Task AddUser(string circuitId)
        {
            _connectedCircuits.TryAdd(circuitId, true);
            NotifyClients();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Removes a user when a circuit (connection) is closed.
        /// </summary>
        public Task RemoveUser(string circuitId)
        {
            _connectedCircuits.TryRemove(circuitId, out _);
            NotifyClients();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the current number of connected users.
        /// </summary>
        public int GetUserCount()
        {
            return _connectedCircuits.Count;
        }

        /// <summary>
        /// Notifies all subscribed clients about the updated user count.
        /// </summary>
        private void NotifyClients()
        {
            int count = _connectedCircuits.Count;
            Console.WriteLine($"🔄 [UserTrackerService] Updated user count: {count}");
            OnUserCountChanged?.Invoke(count);
        }
    }
}
