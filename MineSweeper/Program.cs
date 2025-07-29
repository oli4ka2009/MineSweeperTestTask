using MineSweeper.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IGameBoardFactory, GameBoardFactory>();
builder.Services.AddScoped<IGameService, GameService>();

builder.Services.AddDistributedMemoryCache(); // Додає кеш в пам'яті для зберігання сесії

builder.Services.AddSession(options =>
{
    // Встановлюємо час життя сесії. Якщо користувач неактивний, сесія зникне.
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true; // Робимо cookie недоступним для скриптів на клієнті
    options.Cookie.IsEssential = true; // Важливо для відповідності GDPR
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Game}/{action=Index}/{id?}");

app.Run();
