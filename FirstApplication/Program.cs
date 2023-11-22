using BookShop.Abstract;
using BookShop.Concreate;
using BookShop.Db;
using BookShop.Entities;
using BookShop.Models.EmailSender;
using BookShop.Models.RequestModels;
using BookShop.Seed;
using BookShop.Services;
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
builder.Services.AddDbContext<AppDbContext>();

#endregion

#region Identity

// 2. Identity
builder.Services.AddIdentity<User, Role>(p =>
{
    p.Password.RequiredLength = 8;
    p.Password.RequireNonAlphanumeric = true;
    p.Password.RequireDigit = true;
    p.Password.RequireLowercase = true;
    p.Password.RequireUppercase = true;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddUserManager<UserManager<User>>()
    .AddSignInManager<SignInManager<User>>();


#endregion

#region Jwt

// 3. Adding Authentication
builder.Services.AddAuthentication(options =>
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
        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]!))
    };
});

#endregion

#region Scoped

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IRepository<Category>, GenericRepository<Category>>();
builder.Services.AddScoped<IRepository<Author>, GenericRepository<Author>>();
builder.Services.AddScoped<IRepository<Book>, GenericRepository<Book>>();
builder.Services.AddScoped<IRepository<Branch>, GenericRepository<Branch>>();
builder.Services.AddScoped<IRepository<Invoice>, GenericRepository<Invoice>>();
builder.Services.AddScoped<IRepository<Order>, GenericRepository<Order>>();
builder.Services.AddScoped<IRepository<User>, GenericRepository<User>>();
builder.Services.AddScoped<IRepository<BookVersion>, GenericRepository<BookVersion>>();

builder.Services.AddScoped<IValidation<CategoryRequest>, Validation<CategoryRequest>>();

#endregion

#region Validations

builder.Services.AddTransient<IValidator<CategoryRequest>, DataTableReqValidation>();

#endregion

#region EmailSender

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();

#endregion

#region Controller

builder.Services.AddControllers();

//Add Service To the container.
builder.Services.AddControllers().AddNewtonsoftJson(options =>
options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

#endregion

#region Swagger

// 5. Swagger authentication

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(c =>
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
builder.Services.AddCors(options =>
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

//7. Use CORS
app.UseCors("AllowAngularDevClient");

app.UseHttpsRedirection();

// 8. Authentication
app.UseAuthentication();

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
