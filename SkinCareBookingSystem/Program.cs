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
using SkinCareBookingSystem.Binder;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;


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
builder.Services.AddControllers(options =>
{
    options.ModelBinderProviders.Insert(0, new BinderTypeModelBinderProvider());
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
builder.Services.AddScoped<IValidator<CreateBookingDTO>, CreateBookingDTOValidator>();
builder.Services.AddScoped<IValidator<UpdateBookingDTO>, UpdateBookingDTOValidator>();
builder.Services.AddScoped<ITherapistScheduleService, TherapistScheduleService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<ITherapistTimeSlotService, TherapistTimeSlotService>();
builder.Services.AddScoped<IValidator<CreateTherapistTimeSlotDTO>, CreateTherapistTimeSlotDTOValidator>();
builder.Services.AddScoped<IValidator<UpdateTherapistTimeSlotDTO>, UpdateTherapistTimeSlotDTOValidator>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IValidator<CreateFeedbackDTO>, CreateFeedbackDTOValidator>();
builder.Services.AddScoped<IValidator<UpdateFeedbackDTO>, UpdateFeedbackDTOValidator>();





builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JWT Authorization header sử dụng scheme Bearer.",
        Type = SecuritySchemeType.Http,
        Name = "Authorization",
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});


var app = builder.Build();



app.UseCors("AllowAll");


if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
    context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
    context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Authorization");
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = 204;
        await context.Response.CompleteAsync();
        return;
    }
    await next();
});
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
