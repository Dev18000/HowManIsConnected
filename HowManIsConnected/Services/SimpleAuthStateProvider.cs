using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HowManIsConnected.Services
{
    public class SimpleAuthStateProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal _user = new(new ClaimsIdentity());
        private readonly IServiceProvider _serviceProvider;

        public SimpleAuthStateProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(new AuthenticationState(_user));
        }

        public void Login(string email, string name)
        {
            Console.WriteLine($"✅ [SimpleAuthStateProvider] Logging in: {name} ({email})");

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email)
            }, "BlazorAuth");

            _user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_user)));

            Console.WriteLine($"🔄 [SimpleAuthStateProvider] State changed. Notifying Blazor.");

            using var scope = _serviceProvider.CreateScope();
            var userTracker = scope.ServiceProvider.GetRequiredService<UserTrackerService>();
            userTracker.HandleAuthStateChanged().Wait(); 
        }

        public void Logout()
        {
            _user = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_user)));
        }
    }
}
