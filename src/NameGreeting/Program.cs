var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Order matters: UseDefaultFiles only rewrites "/" to "/index.html";
// UseStaticFiles then serves that file. Reversed, "/" would return 404.
app.UseDefaultFiles();
app.UseStaticFiles();

// The server is the only place a name is validated (FR-007), so the page simply
// shows whatever this endpoint returns.
app.MapPost("/api/greeting", (GreetingRequest request) =>
{
    var validation = NameValidator.Validate(request.Name);

    return validation.IsValid
        ? Results.Ok(new GreetingResponse(validation.CleanedName!))
        : Results.BadRequest(new ErrorResponse(validation.Error!));
});

app.Run();

// Shapes of the JSON sent between the page and the server (see data-model.md).
// ASP.NET Core writes these properties in camelCase: "name" and "error".
record GreetingRequest(string? Name);
record GreetingResponse(string Name);
record ErrorResponse(string Error);

// Top-level statements generate an internal Program class. Declaring it public lets
// WebApplicationFactory<Program> in the test project find and start this app.
public partial class Program { }
