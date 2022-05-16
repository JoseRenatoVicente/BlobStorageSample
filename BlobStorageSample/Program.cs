using BlobStorageSample.Services;
using BlobStorageSample.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>();
builder.Services.AddSingleton(builder.Configuration.GetSection("BlobStorage").Get<BlobStorageSettings>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
