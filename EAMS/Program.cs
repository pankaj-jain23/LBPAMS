using EAMS.Helper;
using EAMS.Hubs;
using EAMS.Middleware;
using EAMS_ACore.AuthInterfaces;
using EAMS_ACore.AuthModels;
using EAMS_ACore.IAuthRepository;
using EAMS_ACore.Interfaces;
using EAMS_ACore.IRealTime;
using EAMS_ACore.IRepository;
using EAMS_BLL.AuthServices;
using EAMS_BLL.RealTimeServices;
using EAMS_BLL.Services;
using EAMS_DAL.AuthRepository;
using EAMS_DAL.DBContext;
using EAMS_DAL.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using WebSocketOptions = Microsoft.AspNetCore.Builder.WebSocketOptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpClient();

builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MapperProfile)); // Add your profile class here
builder.Services.AddDbContextPool<EamsContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Postgres");

    options.UseNpgsql(connectionString,
        npgsqlOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
            // Adjust maxRetryCount and maxRetryDelay as needed.
        });
});
builder.Services
    .AddIdentity<UserRegistration, IdentityRole>()
    .AddEntityFrameworkStores<EamsContext>()
    .AddDefaultTokenProviders();


// Config Identity
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 3;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedEmail = false;
});
// Add Authentication and JwtBearer
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true, // Set to true for lifetime validation
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
            ClockSkew = TimeSpan.Zero, // Set to zero or adjust according to your requirements
            RequireSignedTokens = true,

        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                // TODO just test empty token
                if (!string.IsNullOrEmpty(accessToken))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IAuthRepository, AuthRepository>();
builder.Services.AddTransient<IEamsService, EamsService>();
builder.Services.AddTransient<IEamsRepository, EamsRepository>();
builder.Services.AddTransient<INotificationService, NotificationService>();
builder.Services.AddTransient<INotificationRepository, NotificationRepository>();
builder.Services.AddTransient<IUserConnectionService, UserConnectionService>();
builder.Services.AddTransient<IUserConnectionServiceRepository, UserConnectionServiceRepository>();
builder.Services.AddTransient<IRealTime, RealTimeService>();
//builder.Services.AddHostedService<DatabaseListenerService>();

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "LBPAMS-API", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

builder.Services.AddSignalR(options =>
{
    // Enable detailed logging for diagnostic purposes
    options.EnableDetailedErrors = true;
})
    .AddHubOptions<DashBoardHub>(options =>
    {
        options.MaximumReceiveMessageSize = 102400000;
        options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
        options.KeepAliveInterval = TimeSpan.FromMinutes(2);
        options.MaximumParallelInvocationsPerClient = 10;

    });
builder.Services.AddSignalRCore();
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()    
    .CreateLogger();


builder.Logging.AddSerilog(logger);

var app = builder.Build();
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Remove("X-Powered-By");
    context.Response.Headers.Add("Referrer-Policy", "no-referrer");
    context.Response.Headers.Add("Access-Control-Allow-Origin", "*"); // Allow CORS for any origin
    context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization"); // Allow specific headers
    context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE"); // Allow specific methods
    context.Response.Headers.Add("Access-Control-Allow-Credentials", "true"); // Allow credentials for WebSocket
    await next();
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();
app.UseWebSockets();

app.UseAuthentication();
app.UseMiddleware<TokenExpirationMiddleware>();

app.MapHub<DashBoardHub>("/DashBoardHub", options =>
{
}).RequireAuthorization();

app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();


