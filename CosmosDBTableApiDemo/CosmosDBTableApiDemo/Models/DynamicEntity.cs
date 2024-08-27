using Azure;
using Azure.Data.Tables;
using System.Collections.Generic;

namespace CosmosDBTableApiDemo.Models
{
    public class DynamicEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public IDictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

        // Required by ITableEntity
        public DynamicEntity() { }

        public DynamicEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        // Converts properties to a format suitable for Azure Table API
        public IDictionary<string, object> GetProperties()
        {
            var properties = new Dictionary<string, object>
            {
                { nameof(PartitionKey), PartitionKey },
                { nameof(RowKey), RowKey }
            };

            foreach (var prop in Properties)
            {
                properties[prop.Key] = prop.Value;
            }

            return properties;
        }
    }
}
