using Microsoft.AspNetCore.Authentication.Cookies;
using MS_Web.Service;
using MS_Web.Service.IService;
using MS_Web.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Register HTTP Client service
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddHttpClient<ICouponService, CouponService>();
builder.Services.AddHttpClient<IProductService, ProductService>();
builder.Services.AddHttpClient<IAuthService, AuthService>();


//Register Baseservice
builder.Services.AddScoped<IBaseService,BaseService>();
//Register Couponservice
builder.Services.AddScoped<ICouponService,CouponService>();
builder.Services.AddScoped<IProductService,ProductService>();
builder.Services.AddScoped<ICartService,CartService>();
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
    options.LoginPath = "/auth/login";
    options.AccessDeniedPath = "/";
});

SD.CouponAPIBase = builder.Configuration["ServiceUrls:CouponAPIBase"];
SD.AuthAPIBase = builder.Configuration["ServiceUrls:AuthAPIBase"];
SD.ProductAPIBase = builder.Configuration["ServiceUrls:ProductAPIBase"];
SD.CartAPIBase = builder.Configuration["ServiceUrls:CartAPIBase"];


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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
