using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// the MySQL database
var mysql = builder.AddMySql("mysql", port: 3306)
    .WithLifetime(ContainerLifetime.Persistent);

var mysqldb = mysql.AddDatabase("ContentDb");

// the Azure Storage emulator
var storage = builder.AddAzureStorage("storage").RunAsEmulator(azurite =>
{
    azurite.WithLifetime(ContainerLifetime.Persistent);
    azurite.WithBlobPort(10000)
        .WithQueuePort(10001)
        .WithTablePort(10002);
});

var blobs = storage.AddBlobs("blobs");
var container = blobs.AddBlobContainer("shells");

// the admin password
var adminPassword = builder.AddParameter("admin-password", secret: true);

builder.AddProject<OrchardApp>("App")
    .WithEnvironment("OrchardCore__OrchardCore_DataProtection_Azure__ConnectionString", blobs)
    .WithEnvironment("OrchardCore__OrchardCore_Shells_Azure__ConnectionString", blobs)
    .WithEnvironment("OrchardCore__OrchardCore_AutoSetup__Tenants__0__DatabaseConnectionString", mysqldb)
    .WithEnvironment("OrchardCore__OrchardCore_AutoSetup__Tenants__0__AdminPassword", adminPassword)
    .WaitFor(container)
    .WaitFor(mysqldb);


builder.Build().Run();