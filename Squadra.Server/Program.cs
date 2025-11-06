using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Squadra;
using Squadra.Server.Context;
using Squadra.Server.Models;
using Squadra.Server.Repositories;
using Squadra.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IKrajRepository, KrajRepository>();
builder.Services.AddScoped<IKrajService, KrajService>();

builder.Services.AddScoped<IProfilRepository, ProfilRepository>();
builder.Services.AddScoped<IProfilService, ProfilService>();

builder.Services.AddScoped<IUzytkownikRepository, UzytkownikRepository>();
builder.Services.AddScoped<IUzytkownikService, UzytkownikService>();

builder.Services.AddScoped<IStatusRepository, StatusRepository>();
builder.Services.AddScoped<IStatusService, StatusService>();


builder.Services.AddScoped<IRegionRepository, RegionRepository>();
builder.Services.AddScoped<IRegionService, RegionService>();

builder.Services.AddScoped<IJezykRepository, JezykRepository>();
builder.Services.AddScoped<IJezykService, JezykService>();

builder.Services.AddScoped<IStopienBieglosciJezykaRepository, StopienBieglosciJezykaRepository>();
builder.Services.AddScoped<IStopienBieglosciJezykaService, StopienBieglosciJezykaService>();

// ustawiamy Identity
builder.Services
    .AddIdentityCore<Uzytkownik>()
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// Dodajemy autentykację za pomocą cookies i autoryzację
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    // musi się zgadzać schemat IdentityConstants.ApplicationScheme w obu, aby SignInManager i [Authorize] korzystały z tych samych cookies
    .AddCookie(IdentityConstants.ApplicationScheme, options =>
    {
        options.LoginPath = "/api/auth/login";
        options.Cookie.HttpOnly = true;
        // Jeśli front działa pod tym samym originem: Lax zwykle wystarczy
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

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