var builder = WebApplication.CreateBuilder(args);

// ── Register services BEFORE builder.Build() ────────────────────────────────

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("exercisedb", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// ── Build the app ────────────────────────────────────────────────────────────

var app = builder.Build();

// ── Configure the HTTP request pipeline ─────────────────────────────────────

// NOTE: No UseHttpsRedirection — Render handles TLS at the load balancer level.
// Enabling it inside the container causes redirect loops.

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// ── Route — defaults to ExerciseController/Index ─────────────────────────────

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Exercise}/{action=Index}/{id?}");

app.Run();
