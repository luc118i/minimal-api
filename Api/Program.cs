using minimal_api.Api;

IHostBuilder CreateHostBuilder(string[] args)
{
    return Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(WebBuilder =>
    {
        WebBuilder.UseStartup<Startup>();
    });
}

CreateHostBuilder(args).Build().Run();



