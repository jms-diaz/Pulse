var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres", port: 5432)
    .WithDataVolume()
    .AddDatabase("pulse");

var redis = builder.AddRedis("redis");

var api = builder.AddProject<Projects.Pulse_Api>("pulse-api")
    .WithExternalHttpEndpoints()
    .WithReference(postgres)
    .WithReference(redis)
    .WaitFor(postgres)
    .WaitFor(redis);

var frontend = builder.AddViteApp("frontend", "../pulse-frontend")
    .WithReference(api)
    .WithEnvironment("VITE_APP_BASE_URL", api.GetEndpoint("https"))
    .WaitFor(api);

builder.Build().Run();
