using Microsoft.EntityFrameworkCore;
using TodoApi.Exceptions;
using TodoDataAccess;
using TodoDataAccess.Implementation;
using TodoDataAccess.Interfaces;
using TodoLogic.Implementation;
using TodoLogic.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder
    .Services.AddDbContext<TodoContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("TodoContext")))
    .AddScoped<ITodoListRepository, TodoListRepository>()
    .AddScoped<ITodoListService, TodoListService>()
    .AddScoped<ITodoItemRepository, TodoItemRepository>()
    .AddScoped<ITodoItemService, TodoItemService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<CustomExceptionFilter>();
});



builder.Logging.ClearProviders();
builder.Logging.AddConsole();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API v1");
    });
}

app.UseAuthorization();
app.MapControllers();
app.Run();
