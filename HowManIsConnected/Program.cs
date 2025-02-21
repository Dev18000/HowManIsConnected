using HowManIsConnected.Services;
using Microsoft.AspNetCore.Components.Server.Circuits;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
    .AddHubOptions(options =>
    {
        options.EnableDetailedErrors = true;
        options.ClientTimeoutInterval = TimeSpan.FromSeconds(10); 
        options.HandshakeTimeout = TimeSpan.FromSeconds(5); 
    });

builder.Services.AddSingleton<UserTrackerService>();
builder.Services.AddSingleton<UserTrackerCircuitHandler>();
builder.Services.AddSingleton<CircuitHandler>(sp => sp.GetRequiredService<UserTrackerCircuitHandler>());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Setup Blazor Server
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
