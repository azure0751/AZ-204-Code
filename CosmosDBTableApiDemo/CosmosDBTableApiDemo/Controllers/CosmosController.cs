using CosmosDBTableApiDemo.Models;
using CosmosDBTableApiDemo.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CosmosDBTableApiDemo.Controllers
{
    public class CosmosController : Controller
    {
        private readonly CosmosTableService _cosmosTableService;

        public CosmosController(CosmosTableService cosmosTableService)
        {
            _cosmosTableService = cosmosTableService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var entities = await _cosmosTableService.RetrieveAllEntitiesAsync();
            return View(entities);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string connectionString, string tableName, string partitionKey, string rowKey, List<PropertyInputModel> properties)
        {
            _cosmosTableService.Initialize(connectionString, tableName);

            var entity = new DynamicEntity(partitionKey, rowKey);

            foreach (var property in properties)
            {
                object value;
                switch (property.Type)
                {
                    case "int":
                        value = int.TryParse(property.Value, out int intValue) ? intValue : 0;
                        break;
                    case "bool":
                        value = bool.TryParse(property.Value, out bool boolValue) ? boolValue : false;
                        break;
                    case "string":
                    default:
                        value = property.Value;
                        break;
                }
                entity.Properties[property.Key] = value;
            }

            await _cosmosTableService.InsertEntityAsync(entity);

            var entities = await _cosmosTableService.RetrieveAllEntitiesAsync();
            return View(entities);
        }
    }
}
