using Azure;
using EmployeeManagement.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace EmployeeManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class EmployeeController : ControllerBase
    {
        private IConfiguration _configuration;

        // Cosmos DB details, In real use cases, these details should be configured in secure configuraion file.
        private  string CosmosDBAccountUri = "";
        private  string CosmosDBAccountPrimaryKey = "";
        private  string CosmosDbName = "";
        private  string CosmosDbContainerName = "";

        public EmployeeController(IConfiguration config)
        {
            _configuration = config;
            CosmosDBAccountUri = _configuration["CosmosDBSettings:CosmosDBEndPoint"];
            CosmosDBAccountPrimaryKey= _configuration["CosmosDBSettings:CosmosDBAccesKey"];
            CosmosDbName = _configuration["CosmosDBSettings:CosmosDBDataBaseName"];
            CosmosDbContainerName = _configuration["CosmosDBSettings:CosmosDBContainerName"];
           
        }

        /// <summary>
        /// Commom Container Client, you can also pass the configuration paramter dynamically.
        /// </summary>
        /// <returns> Container Client </returns>
        private Container ContainerClient()
        {

            CosmosClient cosmosDbClient = new CosmosClient(CosmosDBAccountUri, CosmosDBAccountPrimaryKey);
            Container containerClient = cosmosDbClient.GetContainer(CosmosDbName, CosmosDbContainerName);
            return containerClient;
           
        }


        [HttpPost]
        public async Task<IActionResult> AddEmployee(EmployeeModel employee)
        {
            try
            {
                var container = ContainerClient();
                var response = await container.CreateItemAsync(employee, new PartitionKey(employee.Department));

                return Ok(response);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
               
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeDetails()
        {
            try
            {
                var container = ContainerClient();
                var sqlQuery = "SELECT * FROM c";
                QueryDefinition queryDefinition = new QueryDefinition(sqlQuery);
                FeedIterator<EmployeeModel> queryResultSetIterator = container.GetItemQueryIterator<EmployeeModel>(queryDefinition);


                List<EmployeeModel> employees = new List<EmployeeModel>();

                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<EmployeeModel> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (EmployeeModel employee in currentResultSet)
                    {
                        employees.Add(employee);
                    }
                }

                return Ok(employees);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
           
        }
        [HttpGet]
        public async Task<IActionResult> GetEmployeeDetailsById(string employeeId,string partitionKey)
        {

            try
            {
                var container = ContainerClient();
                ItemResponse<EmployeeModel> response = await container.ReadItemAsync<EmployeeModel>(employeeId, new PartitionKey(partitionKey));
                return Ok(response.Resource);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }
        [HttpPut]
        public async Task<IActionResult> UpdateEmployee(EmployeeModel emp,string partitionKey)
        {

            try
            {

                var container = ContainerClient();
                ItemResponse<EmployeeModel> res = await container.ReadItemAsync<EmployeeModel>(emp.id, new PartitionKey(partitionKey));

                //Get Existing Item
                var existingItem = res.Resource;

                //Replace existing item values with new values 
                existingItem.Name = emp.Name;
                existingItem.Country = emp.Country;
                existingItem.City = emp.City;
                existingItem.Department = emp.Department;
                existingItem.Designation = emp.Designation;

              var updateRes=  await container.ReplaceItemAsync(existingItem, emp.id, new PartitionKey(partitionKey));

                return Ok(updateRes.Resource);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
          
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteEmployee(string empId, string partitionKey)
        {

            try
            {

                var container = ContainerClient();
               var response= await container.DeleteItemAsync<EmployeeModel>(empId, new PartitionKey(partitionKey));
                return Ok(response.StatusCode);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateSampleDB()
        {

            try
            {
                CosmosClient cosmosClient = new CosmosClient(CosmosDBAccountUri, CosmosDBAccountPrimaryKey, new CosmosClientOptions() { AllowBulkExecution = true });
                // </CreateClient>

                // <Initialize>

                Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(CosmosDbName);

                // Configure indexing policy to exclude all attributes to maximize RU/s usage
                Console.WriteLine($"Creating a container if not already exists...");
              var response=  await database.DefineContainer(CosmosDbContainerName, "/Department")
                        .WithIndexingPolicy()
                            .WithIndexingMode(IndexingMode.Consistent)
                            .WithIncludedPaths()
                                .Attach()
                            .WithExcludedPaths()
                                .Path("/*")
                                .Attach()
                        .Attach()
                    .CreateAsync();

                return Ok(response.Resource);

            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
 
        }

        [HttpGet]
        public async Task<IActionResult> CheckRequiredConfigValues()
        {

            try
            {
                ConfigurationSettings configsettings = new ConfigurationSettings();
                configsettings.messsage = "make sure these configuration exists and have current values.";
                if (string.IsNullOrEmpty(CosmosDBAccountUri))
                {
                    configsettings.CosmosDBSettingsCosmosDBEndPoint = "CosmosDBSettings:CosmosDBEndPoint Not found";
                }
                else
                {
                    configsettings.CosmosDBSettingsCosmosDBEndPoint = "CosmosDBSettings:CosmosDBEndPoint Exists";
                }

                if (string.IsNullOrEmpty(CosmosDBAccountPrimaryKey))
                {
                    configsettings.CosmosDBSettingsCosmosDBAccesKey = "CosmosDBSettings:CosmosDBAccesKey Not found";
                }
                else
                {
                    configsettings.CosmosDBSettingsCosmosDBAccesKey = "CosmosDBSettings:CosmosDBAccesKey Exists";
                }

                if (string.IsNullOrEmpty(CosmosDbName))
                {
                    configsettings.CosmosDBSettingsCosmosDBDataBaseName = "CosmosDBSettings:CosmosDBDataBaseName Not found";
                }
                else
                {
                    configsettings.CosmosDBSettingsCosmosDBDataBaseName = "CosmosDBSettings:CosmosDBDataBaseName Exists";
                }

                if (string.IsNullOrEmpty(CosmosDbContainerName))
                {
                    configsettings.CosmosDBSettingsCosmosDBContainerName = "CosmosDBSettings:CosmosDBContainerName Not found";
                }
                else
                {
                    configsettings.CosmosDBSettingsCosmosDBContainerName = "CosmosDBSettings:CosmosDBContainerName Exists";
                }

                return Ok(configsettings);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

    }
}