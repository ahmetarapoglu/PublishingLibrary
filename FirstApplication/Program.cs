using BookShop.Abstract;
using BookShop.Concreate;
using BookShop.Db;
using BookShop.Entities;
using BookShop.Middleware;
using BookShop.Models.EmailSender;
using BookShop.Models.RequestModels;
using BookShop.Seed;
using BookShop.Validations.ReqValidation;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var service = builder.Services;

#region DatabaseConnection

// 1. DbContext
service.AddDbContext<AppDbContext>();

#endregion

#region Identity

// 2. Identity
service.AddIdentity<User, Role>(options =>
{
    //User
    options.User.RequireUniqueEmail = true;
    // Password settings
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    // Lockout settings
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
    options.Lockout.MaxFailedAccessAttempts = 3;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddUserManager<UserManager<User>>()
    .AddSignInManager<SignInManager<User>>();


#endregion

#region Jwt

// 3. Adding Authentication
service.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})

// 4. Adding Jwt Bearer
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]!))
    };
});

#endregion

#region Scoped

service.AddScoped<IRepository<Category>, GenericRepository<Category>>();
service.AddScoped<IRepository<Author>, GenericRepository<Author>>();
service.AddScoped<IRepository<Book>, GenericRepository<Book>>();
service.AddScoped<IRepository<Branch>, GenericRepository<Branch>>();
service.AddScoped<IRepository<Invoice>, GenericRepository<Invoice>>();
service.AddScoped<IRepository<Order>, GenericRepository<Order>>();
service.AddScoped<IRepository<User>, GenericRepository<User>>();
service.AddScoped<IRepository<BookVersion>, GenericRepository<BookVersion>>();

service.AddScoped<IAccountRepository<User>, AccountRepository<User>>();

#endregion

#region EmailSender

service.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
service.AddTransient<IEmailService, EmailService>();

#endregion

#region Controller

service.AddControllers();

//Add Service To the container.
service.AddControllers().AddNewtonsoftJson(options =>
options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

#endregion

#region Swagger

// 5. Swagger authentication

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
service.AddEndpointsApiExplorer();


service.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Wedding Planner API", Version = "v1" });
    var SecurityScheme = new OpenApiSecurityScheme
    {
        Description = "Using the Authorization header with the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    c.AddSecurityDefinition("Bearer", SecurityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {SecurityScheme , new[]{ "Bearer"} }
    });
});

#endregion

#region Cors

// 6. Add CORS policy
service.AddCors(options =>
{
    options.AddPolicy("AllowAngularDevClient",
        b =>
        {
            b
                //.WithOrigins("https://nextauth-sage.vercel.app")
                //.WithOrigins("https://bookshop-theta.vercel.app")
                .WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowCredentials()
                .AllowAnyMethod()
                .WithMethods("POST", "GET", "PUT", "DELETE");
        });
});

#endregion

#region Builder
var app = builder.Build();

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await DbInitializer.InitializerAsync(app);
    app.UseSwagger();
    app.UseSwaggerUI();
}

//7.Use CORS
app.UseCors("AllowAngularDevClient");

app.UseHttpsRedirection();

// 8.Use Authentication
app.UseAuthentication();

// 9.Use Middleware
app.UseMiddleware<MyMiddleware>();

app.UseAuthorization();

var cacheMaxAgeOneWeek = (60 * 60 * 24 * 7).ToString();
app.UseStaticFiles(new StaticFileOptions
{
    //FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "wwwroot/Upload/Files")),
    //RequestPath = "/files",

    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append(
                "Cache-Control", $"public, max-age={cacheMaxAgeOneWeek}");
    }
});

app.MapControllers();

#endregion

app.Run();
