using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.Implements;
using SkinCareBookingSystem.Interfaces;
using SkinCareBookingSystem.Validators;
using System.Text;
using SkinCareBookingSystem.DTOs;


var builder = WebApplication.CreateBuilder(args);

var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "MySuperSecureDefaultKey!");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserDTOValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginDTOValidator>();
builder.Services.AddScoped<IUserDetailsService, UserDetailsService>();
builder.Services.AddScoped<IValidator<CreateUserDetailsDTO>, CreateUserDetailsDTOValidator>();
builder.Services.AddScoped<IValidator<UpdateUserDetailsDTO>, UpdateUserDetailsDTOValidator>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IValidator<CreateRoleDTO>, CreateRoleDTOValidator>();
builder.Services.AddScoped<IValidator<UpdateRoleDTO>, UpdateRoleDTOValidator>();
builder.Services.AddScoped<IServiceCategoryService, ServiceCategoryService>();
builder.Services.AddScoped<IValidator<CreateServiceCategoryDTO>, CreateServiceCategoryDTOValidator>();
builder.Services.AddScoped<IValidator<UpdateServiceCategoryDTO>, UpdateServiceCategoryDTOValidator>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IValidator<CreateServiceDTO>, CreateServiceDTOValidator>();
builder.Services.AddScoped<IValidator<UpdateServiceDTO>, UpdateServiceDTOValidator>();
builder.Services.AddScoped<IQaService, QaService>();
builder.Services.AddScoped<IValidator<CreateQaDTO>, CreateQaDTOValidator>();
builder.Services.AddScoped<IValidator<UpdateQaDTO>, UpdateQaDTOValidator>();
builder.Services.AddScoped<IQaAnswerService, QaAnswerService>();
builder.Services.AddScoped<IValidator<CreateQaAnswerDTO>, CreateQaAnswerDTOValidator>();
builder.Services.AddScoped<IValidator<UpdateQaAnswerDTO>, UpdateQaAnswerDTOValidator>();
builder.Services.AddScoped<ICartItemService, CartItemService>();
builder.Services.AddScoped<IValidator<CreateCartItemDTO>, CreateCartItemDTOValidator>();
builder.Services.AddScoped<IValidator<UpdateCartItemDTO>, UpdateCartItemDTOValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateBookingDTOValidator>();
builder.Services.AddScoped<IValidator<CreateBookingDTO>, CreateBookingDTOValidator>();
builder.Services.AddScoped<IValidator<UpdateBookingDTO>, UpdateBookingDTOValidator>();
builder.Services.AddScoped<ITherapistScheduleService, TherapistScheduleService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IValidator<CreateBookingDTO>, CreateBookingDTOValidator>();
builder.Services.AddScoped<IValidator<UpdateBookingDTO>, UpdateBookingDTOValidator>();






builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {your_token}' to access secured endpoints."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();



app.UseCors("AllowAll");


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
