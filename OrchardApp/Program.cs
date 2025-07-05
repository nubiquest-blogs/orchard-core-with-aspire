using OrchardApp.Migrations;
using OrchardCore.Data.Migration;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();


builder.Services
    .AddOrchardCms()
    .ConfigureServices(services =>
    {
        services.AddDataMigration<ImportMigration>();
    })
    .Configure((app, routes, sp) => { })
    .AddAzureShellsConfiguration()
    .AddSetupFeatures("OrchardCore.AutoSetup")
    .EnableFeature("OrchardCore.ContentTypes")
    .EnableFeature("OrchardCore.Contents")
    .EnableFeature("OrchardCore.DataProtection.Azure")
    .EnableFeature("OrchardCore.Recipes")
    .EnableFeature("OrchardCore.Recipes.Core");


var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseOrchardCore(_ =>
{
    // serilog is creating issues
    //c.UseSerilogTenantNameLogging();
});


app.Run();