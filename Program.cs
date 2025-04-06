using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Ajouter les services Razor Pages
builder.Services.AddRazorPages();

// Ajouter les services nécessaires pour les contrôleurs d'API
builder.Services.AddControllers();  // Cette ligne est nécessaire pour les API

// 🔥 AJOUT DE CORS 🔥
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularAndGithub",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200", "https://totommm5.github.io") // Autorise Angular
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });

});

var app = builder.Build();

// Configurer le pipeline des requêtes HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Img")),
    RequestPath = "/Img"
});

app.UseRouting();

// 🔥 ACTIVE CORS 🔥
app.UseCors("AllowAngularAndGithub");

app.UseAuthorization();

// Mapper les pages Razor
app.MapRazorPages();

// Mapper les contrôleurs API
app.MapControllers();

app.Run();
