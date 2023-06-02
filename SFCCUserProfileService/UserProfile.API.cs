using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;

namespace SFCCUserProfileService
{
    public static class GetUserProfile
    {
        [FunctionName("UserProfiles")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "delete", "put", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            var newconfiguration = new ConfigurationBuilder()
                                    .SetBasePath(context.FunctionAppDirectory)
                                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                                    .AddJsonFile("localSecrets.settings.json", optional: true, reloadOnChange: true)
                                    .AddEnvironmentVariables()
                                    .Build();

           
            if ( req.Method == "GET" )
            {
                string id = req.Query["id"];


                // New instance of CosmosClient class
                using CosmosClient client = new CosmosClient(newconfiguration.GetSection("CosmosDBConnectionString").Value);
               
                // Database reference with creation if it does not already exist
                Database database = client.GetDatabase(id: "user_profile");


                // Container reference with creation if it does not alredy exist
                Microsoft.Azure.Cosmos.Container container = database.GetContainer(id: "id");

                QueryDefinition query;
                // Create query using a SQL string and parameters
                if (id == null || id == string.Empty)
                {
                    query = new QueryDefinition(
                                query: "SELECT * FROM id"
                        );
                }
                else
                {
                    query = new QueryDefinition(
                               query: "SELECT * FROM id WHERE id.id = @id"
                       )
                   .WithParameter("@id", id);
                }


                using FeedIterator<UserProfile> feed = container.GetItemQueryIterator<UserProfile>(
                    queryDefinition: query
                );

                List<UserProfile> users = new List<UserProfile>();

                while (feed.HasMoreResults)
                {
                    FeedResponse<UserProfile> response = await feed.ReadNextAsync();
                    foreach (var item in response)
                    {
                        users.Add(item);
                    }

                }


                return new OkObjectResult(users);
            }
            else if ( req.Method == "POST")
            {
                List<UserProfile> users = new List<UserProfile>();

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                UserProfile data = JsonConvert.DeserializeObject<UserProfile>(requestBody);
                string id = data?.id;
                string first_name = data?.first_name;
                string last_name = data?.last_name;
                string billing_zipcode = data?.billing_zipcode;
                string billing_address = data?.billing_address;
                string email = data?.email;
                string billing_state = data?.billing_state;


                // New instance of CosmosClient class
                using CosmosClient client = new CosmosClient(newconfiguration.GetSection("CosmosDBConnectionString").Value);

                // Database reference with creation if it does not already exist
                Database database = client.GetDatabase(id: "user_profile");


                // Container reference with creation if it does not alredy exist
                Microsoft.Azure.Cosmos.Container container = database.GetContainer(id: "id");

                UserProfile r = new UserProfile()
                {
                   id = id,
                   first_name = first_name,
                   last_name = last_name,
                   billing_zipcode = billing_zipcode,
                   billing_address = billing_address,
                   email = email,
                   billing_state = billing_state
                };
                                  

                UserProfile item = await container.CreateItemAsync<UserProfile>(
                       item: r,
                       partitionKey: new PartitionKey(r.id.ToString())
                   );

                return new OkObjectResult(data);
            }
            else if (req.Method == "DELETE")
            {
                try
                {
                    List<UserProfile> users = new List<UserProfile>();

                    string id = req.Query["id"];

                    using CosmosClient client = new CosmosClient(newconfiguration.GetSection("CosmosDBConnectionString").Value);
                    Database database = client.GetDatabase(id: "user_profile");


                    Microsoft.Azure.Cosmos.Container container = database.GetContainer(id: "id");
                    //ResponseMessage deleteResponse = await container.DeleteAllItemsByPartitionKeyStreamAsync(new PartitionKey("Contoso"));

                    // Delete an item. Note we must provide the partition key value and id of the item to delete
                    ItemResponse<UserProfile> userResponse = await container.DeleteItemAsync<UserProfile>(id, new PartitionKey(id));
                    Console.WriteLine("Deleted Family  partitionKey and id [{0},{1}]\n", id, id);

                    return new OkObjectResult(new { message = "Item is deleted" });
                }
                catch (Exception e)
                {
                    var error = new { error = e.Message };
                    return new ContentResult()
                    {
                        Content = e.Message,
                        ContentType = "appliation/json",
                        StatusCode = 503

                    };
                }
               
            }
            else if ( req.Method == "PUT" )
            {
                try
                {
                    using CosmosClient client = new CosmosClient(newconfiguration.GetSection("CosmosDBConnectionString").Value);
                    Database database = client.GetDatabase(id: "user_profile");
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                    UserProfile data = JsonConvert.DeserializeObject<UserProfile>(requestBody);
                    string id = data?.id;
                    string first_name = data?.first_name;
                    string last_name = data?.last_name;
                    string billing_zipcode = data?.billing_zipcode;
                    string billing_address = data?.billing_address;
                    string email = data?.email;
                    string billing_state = data?.billing_state;

                    Microsoft.Azure.Cosmos.Container container = database.GetContainer(id: "id");

                    ItemResponse<UserProfile> user = await container.ReadItemAsync<UserProfile>(id, new PartitionKey(id));
                    var itemBody = user.Resource;

                    // update FirstName
                    itemBody.first_name = first_name == null ? itemBody.first_name : first_name;
                    itemBody.last_name = last_name == null ? itemBody.last_name : last_name;
                    itemBody.email = email == null ? itemBody.email : email;
                    itemBody.billing_address = billing_address == null ? itemBody.billing_address : billing_address;
                    itemBody.billing_state = billing_state == null ? itemBody.billing_state : billing_state;
                    itemBody.billing_zipcode = billing_zipcode == null ? itemBody.billing_zipcode : billing_zipcode;


                    // replace/update the item with the updated content
                    ItemResponse<UserProfile> newUser = await container.ReplaceItemAsync<UserProfile>(itemBody, itemBody.id, new PartitionKey(itemBody.id));

                    return new OkObjectResult(itemBody);
                }
                catch (Exception e)
                {
                    var error = new { error = e.Message };
                    return new ContentResult()
                    {
                        Content = e.Message,
                        ContentType = "appliation/json",
                        StatusCode = 503

                    };
                }
            }
            else
            {
                return new OkObjectResult(null);

            }

        }
    }
}
