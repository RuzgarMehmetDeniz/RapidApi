var builder = WebApplication.CreateBuilder(args);

// 1. Standart MVC servislerini ekle
builder.Services.AddControllersWithViews();

// 2. IHttpClientFactory servisini sisteme tanýt (Hatanýn asýl çözümü)
builder.Services.AddHttpClient();

var app = builder.Build();

// HTTP pipeline yapýlandýrmasý
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Route yapýlandýrmasý
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Hotel}/{action=Index}/{id?}");

app.Run();