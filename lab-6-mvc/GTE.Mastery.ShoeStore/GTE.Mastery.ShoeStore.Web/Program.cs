using AutoMapper;
using FluentValidation;
using GTE.Mastery.ShoeStore.Business.Configurations;
using GTE.Mastery.ShoeStore.Business.Profiles;
using GTE.Mastery.ShoeStore.Business.Dtos;
using GTE.Mastery.ShoeStore.Business.Validators;
using GTE.Mastery.ShoeStore.Data;
using GTE.Mastery.ShoeStore.Domain.Entities;
using GTE.Mastery.ShoeStore.Business.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GTE.Mastery.ShoeStore.Web.Infrastructure;
using GTE.Mastery.ShoeStore.Business.Interfaces;
using GTE.Mastery.ShoeStore.Business.Services;
using GTE.Mastery.ShoeStore.Data.Interfaces;
using GTE.Mastery.ShoeStore.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<MainDbContext>(options => 
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
           .UseLazyLoadingProxies());

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<MainDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddSession();

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new UserProfile());
    mc.AddProfile(new ShoeProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddScoped<IValidator<CreateEditShoeDto>, ShoeDtoValidator>();
builder.Services.AddScoped<IValidator<RegisterDto>, RegisterValidator>();
builder.Services.AddScoped<IValidator<LoginDto>, LoginValiadator>();

builder.Services.AddScoped<IRepository<Shoe>>(s =>
    new Repository<Shoe>(s.GetRequiredService<MainDbContext>()));

builder.Services.AddScoped<IShoesRepository, ShoesRepository>();

builder.Services.AddScoped<IUnitOfWork>(u => 
    new UnitOfWork(u.GetRequiredService<MainDbContext>()));

builder.Services.AddScoped<IShoeService, ShoeService>();
builder.Services.AddScoped<IUserService>( u =>
    new UserService(configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
