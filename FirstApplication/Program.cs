using BookShop.Abstract;
using BookShop.Concreate;
using BookShop.Db;
using BookShop.Entities;
using BookShop.Models.RequestModels;
using BookShop.Seed;
using BookShop.Validations.ReqValidation;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;


// 1. DbContext
builder.Services.AddDbContext<AppDbContext>();

#region Identity

// 2. Identity
builder.Services.AddIdentity<User, Role>(c =>
{
    c.Password.RequiredLength = 8;
    c.Password.RequireNonAlphanumeric = true;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddUserManager<UserManager<User>>()
    .AddSignInManager<SignInManager<User>>();


#endregion

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
        };
    });

#region Scoped

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IRepository<Category>, GenericRepository<Category>>();

#endregion

#region Validator

builder.Services.AddTransient<IValidator<CategoryRequest>, DataTableReqValidation>();

#endregion

builder.Services.AddControllers();



//Add Service To the container.
builder.Services.AddControllers().AddNewtonsoftJson(options =>
options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

//builder.Services.AddControllers()
//        .AddDataAnnotationsLocalization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


// 5. Swagger authentication
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
                .WithMethods("POST","GET","PUT","DELETE");
        });
});
#endregion

var app = builder.Build();

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await DbInitializer.InitializerAsync(app);
    app.UseSwagger();
    app.UseSwaggerUI();
}
//await DbInitializer.InitializerAsync(app);
//app.UseSwagger();
//app.UseSwaggerUI();

//7. Use CORS
app.UseCors("AllowAngularDevClient");
app.UseHttpsRedirection();

// 8. Authentication
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
