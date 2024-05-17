using ElmTest.Application;
using ElmTest.Domain.Interfaces;
using ElmTest.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithOrigins("http://localhost:4200"));
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
});
var elasticsearchUri = new Uri(builder.Configuration["ElasticsearchSettings:Uri"]);

// Configure other services
builder.Services.AddTransient<IBookRepository>(provider =>
    new BookRepository(elasticsearchUri,builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddTransient<BookService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("CorsPolicy");


app.UseAuthorization();

app.MapControllers();

app.Run();
