using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Squadra;
using Squadra.Server.Context;
using Squadra.Server.Models;
using Squadra.Server.Modules.Powiadomienia;
using Squadra.Server.Modules.Profil;
using Squadra.Server.Modules.Uzytkownik;
using Squadra.Server.Modules.Wiadomosci;
using Squadra.Server.Modules.Znajomi;
using Squadra.Server.Repositories;
using Squadra.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// rejestrujemy nasze moduły
builder.Services.AddPowiadomieniaModule();
builder.Services.AddProfilModule();
builder.Services.AddUzytkownikModule();
builder.Services.AddWiadomosciModule();
builder.Services.AddZnajomiModule();

// ustawiamy Identity
builder.Services
    .AddIdentity<Uzytkownik, IdentityRole<int>>()
    // mówi Identity, żeby używać EF Core jako magazynu użytkowników/rol.
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders(); 

// konfigurujemy cookie
builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/api/auth/login";
        options.Cookie.HttpOnly = true;
        // kontroluje, kiedy przeglądarka wysyła cookie do serwera w kontekście cross-site requests
        // (czyli np. jeśli front i back są na różnych domenach/subdomenach).
        
        // my mamy lax, co się ustawia jak jest na tym samym originie
        options.Cookie.SameSite = SameSiteMode.Lax;
        
        // jak dostajemy https, odsyłamy https, http też http
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

        // te dwa następne: pierwsze mówi że traci ważność, drugie - po jakim czasie
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);

        // API nie ma widoku odmowy dostępu, zwracamy 403 zamiast redirecta
        options.Events = new CookieAuthenticationEvents
        {
            // aby nie wywalało błędów w parsowaniu jsona. Sami będziemy przekierowywać
            OnRedirectToLogin = ctx =>
            {
                if (ctx.Request.Path.StartsWithSegments("/api"))
                {
                    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                }
                ctx.Response.Redirect(ctx.RedirectUri);
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = ctx =>
            {
                if (ctx.Request.Path.StartsWithSegments("/api"))
                {
                    ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                }
                ctx.Response.Redirect(ctx.RedirectUri);
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();

// wyrażamy zgodę na ciasteczka z naszej witryny na porcie 3000
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    // jest pod adresem /swagger
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// dodajemy role
using (var scope = app.Services.CreateScope())
{
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<Uzytkownik>>();

    string[] roles = ["Uzytkownik", "Admin"];
    foreach (var role in roles)
    {
        if (!await roleMgr.RoleExistsAsync(role))
            await roleMgr.CreateAsync(new IdentityRole<int>(role));
    }

    // nie chcę jakoś wstawiać swojego emaila tutaj... nie musi być prawdziwy raczej, najwyżej zmienię
    var adminEmail = "admin.legit@squadra.com";
    var admin = await userMgr.FindByEmailAsync(adminEmail);
    if (admin is null)
    {
        admin = new Uzytkownik
        {
            UserName = "admin",
            Email = adminEmail,
            EmailConfirmed = true
        };
        var create = await userMgr.CreateAsync(admin, "brainImpl0si0nEnergy!");
        if (create.Succeeded)
        {
            await userMgr.AddToRoleAsync(admin, "Admin");
        }
    }
}

app.Run();