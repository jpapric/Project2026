using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Server.Application.Interfaces;
using Server.Domain;
using S7.Net;

namespace Server.Infrastructure.BackgroundServices
{
    public class PlcConnection : BackgroundService
    {
        private readonly ILogger<PlcConnection> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private S7.Net.Plc? _plc;

        public PlcConnection(ILogger<PlcConnection> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            Domain.Plc plcConfig = await FetchPlcConfigurationAsync(stoppingToken);
            if (plcConfig == null) return;

            if (!Enum.TryParse(plcConfig.Cpu, true, out CpuType cpuType))
            {
                _logger.LogWarning("Invalid CPU type '{Cpu}', defaulting to S71500.", plcConfig.Cpu);
                cpuType = CpuType.S71500;
            }

            _plc = new S7.Net.Plc(cpuType, plcConfig.Ip, (short)plcConfig.Rack, (short)plcConfig.Slot);

            await ConnectUntilSuccessAsync(stoppingToken);

            await MonitorConnectionAsync(stoppingToken);

            _plc.Close();
        }

        private async Task ConnectUntilSuccessAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Attempting PLC connection...");
                    await _plc!.OpenAsync(stoppingToken);

                    if (_plc.IsConnected)
                    {
                        _logger.LogInformation("PLC connected successfully.");
                        return; 
                    }
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Connection failed: {Message}. Retrying in 3s...", ex.Message);
                }

                await Task.Delay(3000, stoppingToken);
            }
        }

        private async Task MonitorConnectionAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!_plc!.IsConnected)
                {
                    _logger.LogWarning("PLC connection lost. Reconnecting...");
                    await ConnectUntilSuccessAsync(stoppingToken);
                }

                await Task.Delay(3000, stoppingToken);
            }
        }

        private async Task<Domain.Plc> FetchPlcConfigurationAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var repository = scope.ServiceProvider.GetRequiredService<IServerRepository>();
                    Domain.Plc config = repository.GetPlc();

                    if (config != null && !string.IsNullOrEmpty(config.Ip))
                    {
                        _logger.LogInformation("PLC config loaded — IP: {Ip}, CPU: {Cpu}", config.Ip, config.Cpu);
                        return config;
                    }

                    _logger.LogWarning("Empty PLC config in DB, retrying in 3s...");
                }
                catch (Exception ex)
                {
                    _logger.LogError("Failed to fetch PLC config: {Message}. Retrying in 3s...", ex.Message);
                }

                await Task.Delay(3000, cancellationToken);
            }
            return null;
        }
    }
}