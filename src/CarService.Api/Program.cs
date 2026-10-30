using CarService.Api;
using CarService.Application;
using CarService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi(options => options.AddDocumentTransformer<BearerOpenApiTransformer>());
builder.Services.AddAccountAuthentication();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddInfrastructure();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CarCatalog>();
builder.Services.AddScoped<ComponentService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<TestDriveService>();
var app = builder.Build();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapOpenApi().AllowAnonymous();
app.MapControllers();
await app.Services.InitializeInfrastructureAsync();
await app.RunAsync();

public partial class Program;
