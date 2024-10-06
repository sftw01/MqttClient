using Microsoft.AspNetCore.ResponseCompression;
using MqttClient.Server.Hubs;
using MqttClient.Server.Services;
using MqttClient.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// add services
builder.Services.AddSignalR();

builder.Services.AddSingleton<MqttService>();


var app = builder.Build();


//create scope and get service
var scope = app.Services.CreateScope();
var mqttService = scope.ServiceProvider.GetRequiredService<MqttService>();

await mqttService.PublishMessageAsync(new MqttMessageDto { Topic = "mirek", Payload = "Wiadomosc testowa" });
await mqttService.SubscribeToTopicAsync("mirek");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapControllers();

app.MapHub<HubMqtt>("/hubmqtt");

app.MapFallbackToFile("index.html");


app.Run();
