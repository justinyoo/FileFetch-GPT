using FileFetch.Services;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMvc();
//DIs
builder.Services.AddTransient<IOpenAIService, OpenAIService>();
builder.Services.AddTransient<IChatService, ChatService>();
builder.Services.AddHttpClient(Options.DefaultName, httpClient =>
{
    httpClient.DefaultRequestHeaders.Add("User-Agent", "FileFetch");
});

builder.Services.AddCors(o => o.AddPolicy("ReactPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();

}));
builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "react.ui/build";
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("ReactPolicy");

app.UseAuthorization();
app.MapControllers();
app.UseSpaStaticFiles();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Help}/{id?}");
});
app.UseSpa(spa =>
{
    if (app.Environment.IsDevelopment())
    {
        spa.Options.SourcePath = Path.Join(Directory.GetCurrentDirectory(), "react.ui");
        spa.UseReactDevelopmentServer(npmScript: "start");
    }

});
app.Run();
