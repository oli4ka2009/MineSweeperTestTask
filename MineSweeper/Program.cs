using Microsoft.EntityFrameworkCore;
using MineSweeper.Data;
using MineSweeper.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IGameBoardFactory, GameBoardFactory>();
builder.Services.AddScoped<IDifficultyAnalyzer, DifficultyAnalyzer>();
builder.Services.AddScoped<IMinesweeperSolver, MinesweeperSolver>();
builder.Services.AddScoped<IGameSessionService, GameSessionService>();
builder.Services.AddScoped<IGameResultService, GameResultService>();
builder.Services.AddScoped<IGameFactory, GameFactory>();
builder.Services.AddScoped<IGameStateEvaluator, GameStateEvaluator>();
builder.Services.AddScoped<IHintService, HintService>();
builder.Services.AddScoped<ICellInteractionService, CellInteractionService>();
builder.Services.AddScoped<IGameplayService, GameplayService>();
builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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
