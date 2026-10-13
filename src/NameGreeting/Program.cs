var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Order matters: UseDefaultFiles only rewrites "/" to "/index.html";
// UseStaticFiles then serves that file. Reversed, "/" would return 404.
app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();

// Top-level statements generate an internal Program class. Declaring it public lets
// WebApplicationFactory<Program> in the test project find and start this app.
public partial class Program { }
