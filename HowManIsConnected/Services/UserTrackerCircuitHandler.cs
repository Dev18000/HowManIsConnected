using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace HowManIsConnected.Services
{
    public class UserTrackerCircuitHandler : CircuitHandler
    {
        private readonly UserTrackerService _userTracker;
        private readonly AuthenticationStateProvider _authProvider;

        public UserTrackerCircuitHandler(UserTrackerService userTracker, AuthenticationStateProvider authProvider)
        {
            _userTracker = userTracker;
            _authProvider = authProvider;
        }

        public override async Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            Console.WriteLine($"✅ [Blazor] Circuit opened: {circuit.Id}");

            await Task.Delay(500); 

            var authState = await _authProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            string email = user.FindFirst(c => c.Type == ClaimTypes.Email)?.Value ?? "guest@example.com";
            string name = user.Identity?.Name ?? "Guest User";

            Console.WriteLine($"🔹 [Blazor] User detected in circuit: {name} ({email})");

            await _userTracker.AddUser(circuit.Id, email, name);
        }

        public override async Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            Console.WriteLine($"❌ [Blazor] Circuit closed: {circuit.Id}");
            await _userTracker.RemoveUser(circuit.Id);
        }
    }
}
