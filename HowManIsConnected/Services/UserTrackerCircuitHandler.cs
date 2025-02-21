using Microsoft.AspNetCore.Components.Server.Circuits;

namespace HowManIsConnected.Services
{
    /// <summary>
    /// Tracks user circuits and passes authentication info.
    /// </summary>
    public class UserTrackerCircuitHandler : CircuitHandler
    {
        private readonly UserTrackerService _userTracker;

        public UserTrackerCircuitHandler(UserTrackerService userTracker)
        {
            _userTracker = userTracker;
        }

        public override async Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            Console.WriteLine($"✅ [Blazor] Circuit opened: {circuit.Id}");

            // Here you should get user info from authentication
            string email = "guest@example.com"; // Replace with actual authentication logic
            string name = "Guest User"; // Replace with actual authentication logic

            await _userTracker.AddUser(circuit.Id, email, name);
        }

        public override async Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            Console.WriteLine($"❌ [Blazor] Circuit closed: {circuit.Id}");
            await _userTracker.RemoveUser(circuit.Id);
        }
    }
}
