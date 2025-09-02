using Microsoft.Extensions.Options;
using TravelExpenseTracker.Domain;

namespace TravelExpenseTracker.Services
{
    public interface IContainerNameResolver 
    {
        // Maps a logical key (e.g., "Receipts") to the actual container name from configuration.
        string Resolve(string logicalKey);
    }

    public class ContainerNameResolver : IContainerNameResolver
    {
        private readonly AzureBlobContainers _containers;

        public ContainerNameResolver(IOptions<AzureBlobContainers> options)
        {
            _containers = options.Value;
        }
        public string Resolve(string logicalKey)
        {
            return logicalKey switch
            {
                nameof(AzureBlobContainers.Receipts) => _containers.Receipts,
                nameof(AzureBlobContainers.Reports) => _containers.Reports,
                _ => throw new KeyNotFoundException($"Unknown container key {logicalKey}")
            };
        }
    }
}
