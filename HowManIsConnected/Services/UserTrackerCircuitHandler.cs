using Microsoft.AspNetCore.Components.Server.Circuits;

namespace HowManIsConnected.Services
{
    /// <summary>
    /// Tracks user connections using Blazor Server Circuits.
    /// </summary>
    public class UserTrackerCircuitHandler : CircuitHandler
    {
        private readonly UserTrackerService _userTracker;

        public UserTrackerCircuitHandler(UserTrackerService userTracker)
        {
            _userTracker = userTracker;
        }

        /// <summary>
        /// Called when a new user connects to the Blazor Server application.
        /// </summary>
        public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            Console.WriteLine($"✅ [Blazor] Circuit opened: {circuit.Id}");
            return _userTracker.AddUser(circuit.Id);
        }

        /// <summary>
        /// Called when a user disconnects from the Blazor Server application.
        /// </summary>
        public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            Console.WriteLine($"❌ [Blazor] Circuit closed: {circuit.Id}");
            return _userTracker.RemoveUser(circuit.Id);
        }
    }
}
