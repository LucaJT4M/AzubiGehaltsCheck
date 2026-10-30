var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.AzubiGehaltsCheck_Api>("api");

// React + Vite frontend
var frontend = builder.AddViteApp("frontend", "../frontend")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();