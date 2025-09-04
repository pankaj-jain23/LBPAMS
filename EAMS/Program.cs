using EAMS.Helper;
using EAMS.Hubs;
using EAMS.Middleware;
using EAMS_ACore.AuthInterfaces;
using EAMS_ACore.AuthModels;
using EAMS_ACore.IAuthRepository;
using EAMS_ACore.IExternal;
using EAMS_ACore.Interfaces;
using EAMS_ACore.IRealTime;
using EAMS_ACore.IRepository;
using EAMS_BLL.AuthServices;
using EAMS_BLL.ExternalServices;
using EAMS_BLL.RealTimeServices;
using EAMS_BLL.Services;
using EAMS_DAL.AuthRepository;
using EAMS_DAL.DBContext;
using EAMS_DAL.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer(); 

builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MapperProfile)); // Add your profile class here
builder.Services.AddDbContextPool<EamsContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Postgres");

    options.UseNpgsql(connectionString,
        npgsqlOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);

        });
});
var redisConnectionString = builder.Configuration.GetConnectionString("RedisCacheUrl");
builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
{
    var configuration = ConfigurationOptions.Parse(redisConnectionString, true);
    configuration.AbortOnConnectFail = false; // 🔥 important
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services
    .AddIdentity<UserRegistration, IdentityRole>()
    .AddEntityFrameworkStores<EamsContext>()
    .AddDefaultTokenProviders();


// Config Identity
builder.Services.Configure<IdentityOptions>(options =>
{ // ✅ Configure allowed username characters
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.SignIn.RequireConfirmedEmail = true;
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
        //options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true, // Set to true for lifetime validation
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
            ClockSkew = TimeSpan.Zero,

        };
        // Enable JWT Bearer token authentication for SignalR connections
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                // If the request is for our SignalR hub, extract the token from the query string
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/DashBoardHub")))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };

    });

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IEamsService, EamsService>();
builder.Services.AddScoped<IEamsRepository, EamsRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IUserConnectionService, UserConnectionService>();
builder.Services.AddScoped<IUserConnectionServiceRepository, UserConnectionServiceRepository>();
builder.Services.AddScoped<IRealTime, RealTimeService>(); 
builder.Services.AddScoped<ICacheService, CacheService>();
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

builder.Services.AddSignalR();

builder.Services.AddSignalRCore();
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();


builder.Logging.AddSerilog(logger);

builder.Services.AddResponseCompression(o =>
{
    o.EnableForHttps = true;
    o.Providers.Add<BrotliCompressionProvider>();
    o.Providers.Add<GzipCompressionProvider>();
});
builder.Services.Configure<BrotliCompressionProviderOptions>(o =>
{
    o.Level = System.IO.Compression.CompressionLevel.Fastest;
});
builder.Services.Configure<GzipCompressionProviderOptions>(o =>
{
    o.Level = System.IO.Compression.CompressionLevel.Fastest;
});
// If you're using MemoryCache (for in-memory caching):
builder.Services.AddDistributedMemoryCache();
// Configure HttpClient with base address
builder.Services.AddHttpClient("SmsClient", client =>
{
    client.BaseAddress = new Uri("http://10.44.250.220/");
    client.DefaultRequestHeaders.Add("SOAPAction", "http://tempuri.org/SendSMS");
    client.DefaultRequestHeaders.Add("Accept", "application/xml");
})
.ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
{
    MaxConnectionsPerServer = int.MaxValue,
    PooledConnectionLifetime = TimeSpan.FromMinutes(30),
    PooledConnectionIdleTimeout = TimeSpan.FromMinutes(15),
    KeepAlivePingDelay = TimeSpan.FromSeconds(30),
    KeepAlivePingTimeout = TimeSpan.FromSeconds(10),
    KeepAlivePingPolicy = HttpKeepAlivePingPolicy.Always
});


// Register your other services
builder.Services.AddSingleton<IExternal, ExternalService>();

// Register the warmup service
//builder.Services.AddHostedService<SmsClientWarmupService>();
var app = builder.Build();
 

app.UseResponseCompression(); 

app.UseSwagger();
app.UseSwaggerUI();



app.UseCors();
app.UseWebSockets();

app.UseAuthentication();
app.UseMiddleware<TokenExpirationMiddleware>();

app.MapHub<DashBoardHub>("/DashBoardHub", options =>
{
}).RequireAuthorization();
app.UseStaticFiles();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();


