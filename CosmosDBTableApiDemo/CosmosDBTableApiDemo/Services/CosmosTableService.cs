using Azure.Data.Tables;
using CosmosDBTableApiDemo.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDBTableApiDemo.Services
{
    public class CosmosTableService
    {
        private TableClient _tableClient;

        public void Initialize(string connectionString, string tableName)
        {
            var serviceClient = new TableServiceClient(connectionString);
            _tableClient = serviceClient.GetTableClient(tableName);
            _tableClient.CreateIfNotExists();
        }

        public async Task InsertEntityAsync(DynamicEntity entity)
        {
            var tableEntity = new TableEntity(entity.PartitionKey, entity.RowKey);
            foreach (var property in entity.Properties)
            {
                tableEntity[property.Key] = property.Value;
            }
            await _tableClient.AddEntityAsync(tableEntity);
        }

        public async Task<List<DynamicEntity>> RetrieveAllEntitiesAsync()
        {
            var entities = new List<DynamicEntity>();

            if (_tableClient != null)
            {

                await foreach (var tableEntity in _tableClient.QueryAsync<TableEntity>())
                {
                    var entity = new DynamicEntity(tableEntity.PartitionKey, tableEntity.RowKey)
                    {
                        Properties = tableEntity.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                    };

                    entities.Add(entity);
                }
            }
            

            return entities;
        }
    }
}
