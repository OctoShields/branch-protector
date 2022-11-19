using branch_protector.Interfaces;
using branch_protector.Services;
using Octokit.Webhooks;
using Octokit.Webhooks.AspNetCore;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddTransient<IRepositoryService, GitHubRepositoryService>();
        builder.Services.AddSingleton<WebhookEventProcessor, GitHubWebhookEventProcessor>();

        DotNetEnv.Env.Load();


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.MapGitHubWebhooks("/api/repositories/webhook", Environment.GetEnvironmentVariable("GITHUB_WEBHOOK_SECRET_KEY"));

        //app.UseEndpoints(endpoints =>
        //    endpoints.MapGitHubWebhooks("/api/repositories/webhook", Environment.GetEnvironmentVariable("GITHUB_WEBHOOK_SECRET_KEY"))
        //);

        app.Run();
    }
}