var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();

// ── Register services BEFORE builder.Build() ────────────────────────────────

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("exercisedb", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    // If ExerciseDB later requires an API key, add it here:
    // client.DefaultRequestHeaders.Add("x-api-key", builder.Configuration["ExerciseDB:ApiKey"]);
});

// ── Build the app ────────────────────────────────────────────────────────────
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
var app = builder.Build();

// ── Configure the HTTP request pipeline ─────────────────────────────────────

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// ── Single route — defaults to ExerciseController/Index ─────────────────────

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Exercise}/{action=Index}/{id?}");

app.Run();
