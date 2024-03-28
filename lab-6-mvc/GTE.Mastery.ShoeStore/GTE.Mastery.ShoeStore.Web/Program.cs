using AutoMapper;
using FluentValidation;
using GTE.Mastery.ShoeStore.Business.Configurations;
using GTE.Mastery.ShoeStore.Business.Profiles;
using GTE.Mastery.ShoeStore.Business.Dtos;
using GTE.Mastery.ShoeStore.Business.Validators;
using GTE.Mastery.ShoeStore.Data;
using GTE.Mastery.ShoeStore.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GTE.Mastery.ShoeStore.Business.Interfaces;
using GTE.Mastery.ShoeStore.Business.Services;
using GTE.Mastery.ShoeStore.Data.Interfaces;
using GTE.Mastery.ShoeStore.Data.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;

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

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = new PathString("/Error/403");
});

builder.Services.AddSession();

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new UserProfile());
    mc.AddProfile(new ShoeProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddScoped<IValidator<UpdateShoeDto>, ShoeDtoValidator>();
builder.Services.AddScoped<IValidator<RegisterDto>, RegisterValidator>();

builder.Services.AddScoped<IRepository<Shoe>>(s =>
    new Repository<Shoe>(s.GetRequiredService<MainDbContext>()));

builder.Services.AddScoped<IShoesRepository, ShoesRepository>();

builder.Services.AddScoped<IRepositoryFactory<Size>>(r =>
    new RepositoryFactory<Size>(r));

builder.Services.AddScoped<IRepositoryFactory<Brand>>(r =>
    new RepositoryFactory<Brand>(r));

builder.Services.AddScoped<IRepositoryFactory<Category>>(r =>
    new RepositoryFactory<Category>(r));

builder.Services.AddScoped<IRepositoryFactory<Color>>(r =>
    new RepositoryFactory<Color>(r));

builder.Services.AddScoped<IUnitOfWork>(u =>
    new UnitOfWork(u.GetRequiredService<MainDbContext>()));

builder.Services.AddScoped<IShoeService, ShoeService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Error/{0}");

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
