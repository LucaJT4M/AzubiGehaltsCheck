var builder = DistributedApplication.CreateBuilder(args);

// PostgreSQL server
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume();

// Application database
var database = postgres.AddDatabase("appdb");

var api = builder
    .AddProject<Projects.AzubiGehaltsCheck_Api>("api")
    .WithReference(database)
    .WaitFor(database);

// React + Vite frontend
var frontend = builder.AddViteApp("frontend", "../frontend")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();