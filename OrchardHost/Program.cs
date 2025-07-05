using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var adminPassword = builder.AddParameter("admin-password");


var mysql = builder.AddMySql("mysql", port: 3306)
    .WithLifetime(ContainerLifetime.Persistent);

var mysqldb = mysql.AddDatabase("ContentDb");

var storage = builder.AddAzureStorage("storage").RunAsEmulator(azurite =>
{
    azurite.WithLifetime(ContainerLifetime.Persistent);
    azurite.WithBlobPort(10000)
        .WithQueuePort(10001)
        .WithTablePort(10002);
});

var blobs = storage.AddBlobs("blobs");
var container = blobs.AddBlobContainer("shells");

var app = builder.AddProject<OrchardApp>("App")
    .WithReference(blobs)
    .WithReference(mysqldb, "ContentDb")
    .WithEnvironment("OrchardCore__OrchardCore_DataProtection_Azure__ConnectionString", blobs)
    .WithEnvironment("OrchardCore__OrchardCore_Shells_Azure__ConnectionString", blobs)
    .WithEnvironment("OrchardCore__OrchardCore_AutoSetup__Tenants__0__DatabaseConnectionString", mysqldb)
    .WithEnvironment("OrchardCore__OrchardCore_AutoSetup__Tenants__0__AdminPassword", adminPassword)
    .WaitFor(container)
    .WaitFor(mysqldb);


builder.Build().Run();