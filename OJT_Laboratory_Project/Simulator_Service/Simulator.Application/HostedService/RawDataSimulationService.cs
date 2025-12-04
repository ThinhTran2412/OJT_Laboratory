using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Simulator.Application.DTOs;
using Simulator.Application.SimulateRawData.Command;
using Simulator.Application.SimulateRawData.Query; 

namespace Simulator.Application.HostedService
{
    /// <summary>
    /// Handles background service for simulating and sending raw data
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Hosting.BackgroundService" />
    public class RawDataSimulationService : BackgroundService
    {
        /// <summary>
        /// The service provider
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RawDataSimulationService"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public RawDataSimulationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// This method is called when the <see cref="T:Microsoft.Extensions.Hosting.IHostedService" /> starts. The implementation should return a task that represents
        /// the lifetime of the long running operation(s) being performed.
        /// </summary>
        /// <param name="stoppingToken">Triggered when <see cref="M:Microsoft.Extensions.Hosting.IHostedService.StopAsync(System.Threading.CancellationToken)" /> is called.</param>
        /// <remarks>
        /// See <see href="https://learn.microsoft.com/dotnet/core/extensions/workers">Worker Services in .NET</see> for implementation guidelines.
        /// </remarks>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                IEnumerable<RawTestResultDTO> unsentResults = await mediator.Send(new GetUnsentTestResultsQuery());

                if (unsentResults != null && unsentResults.Any())
                {
                    int sentCount = 0;
                    foreach (var rawResult in unsentResults)
                    {
                        await mediator.Send(new SendRawTestResultCommand(rawResult));
                        sentCount++;
                    }
                    Console.WriteLine($"[BACKUP HOST] Sent {sentCount} unsent Test Orders via Queue for backup.");
                }
                else
                {
                    Console.WriteLine("[BACKUP HOST] No unsent test results found in DB. Waiting...");
                }

                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }
        }
    }
}