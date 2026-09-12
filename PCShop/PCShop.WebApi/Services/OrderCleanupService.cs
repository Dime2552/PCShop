using Microsoft.EntityFrameworkCore;
using PCShop.Application.Common.Interfaces;
using PCShop.Domain.Enums;

namespace PCShop.WebApi.Services
{
    public class OrderCleanupService : BackgroundService
    {
        private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan OrderTimeout = TimeSpan.FromMinutes(30);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OrderCleanupService> _logger;

        public OrderCleanupService(
            IServiceScopeFactory scopeFactory,
            ILogger<OrderCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OrderCleanupService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanUpAbandonedOrdersAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while cleaning up abandoned orders.");
                }

                try
                {
                    await Task.Delay(CheckInterval, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }

            _logger.LogInformation("OrderCleanupService is stopping.");
        }

        private async Task CleanUpAbandonedOrdersAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

            var cutoffTime = DateTime.UtcNow.Subtract(OrderTimeout);

            var abandonedOrders = await context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.Status == OrderStatus.Pending && o.CreatedAt < cutoffTime)
                .ToListAsync(cancellationToken);

            if (abandonedOrders.Count == 0)
            {
                return;
            }

            _logger.LogInformation("Found {Count} abandoned order(s) to cancel.", abandonedOrders.Count);

            foreach (var order in abandonedOrders)
            {
                order.Status = OrderStatus.Cancelled;

                foreach (var item in order.Items)
                {
                    if (item.Product != null)
                    {
                        item.Product.StockQuantity += item.Quantity;
                    }
                }
            }

            await context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully cancelled {Count} abandoned order(s) and released reserved inventory.", abandonedOrders.Count);
        }
    }
}
