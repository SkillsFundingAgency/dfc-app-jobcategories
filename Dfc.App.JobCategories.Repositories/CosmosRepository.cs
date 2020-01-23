using Dfc.App.JobCategories.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Dfc.App.JobCategories.Repositories
{
    [ExcludeFromCodeCoverage]
    public class CosmosRepository<T> : ICosmosRepository<T>
        where T : IDataModel
    {
        private readonly CosmosDbConnection cosmosDbConnection;
        private readonly IDocumentClient documentClient;

        public CosmosRepository(CosmosDbConnection cosmosDbConnection, IDocumentClient documentClient, IHostingEnvironment env)
        {
            this.cosmosDbConnection = cosmosDbConnection;
            this.documentClient = documentClient;

            if (env.IsDevelopment())
            {
                CreateDatabaseIfNotExistsAsync().GetAwaiter().GetResult();
                CreateCollectionIfNotExistsAsync().GetAwaiter().GetResult();
            }
        }

        private Uri DocumentCollectionUri => UriFactory.CreateDocumentCollectionUri(cosmosDbConnection.DatabaseId, cosmosDbConnection.CollectionId);

        public async Task<bool> PingAsync()
        {
            var query = documentClient.CreateDocumentQuery<T>(DocumentCollectionUri, new FeedOptions { MaxItemCount = 1, EnableCrossPartitionQuery = true })
                .AsDocumentQuery();

            if (query == null)
            {
                return false;
            }

            var models = await query.ExecuteNextAsync<T>().ConfigureAwait(false);
            var firstModel = models.FirstOrDefault();

            return firstModel != null;
        }

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await documentClient.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(cosmosDbConnection.DatabaseId)).ConfigureAwait(false);
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await documentClient.CreateDatabaseAsync(new Database { Id = cosmosDbConnection.DatabaseId }).ConfigureAwait(false);
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await documentClient.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(cosmosDbConnection.DatabaseId, cosmosDbConnection.CollectionId)).ConfigureAwait(false);
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    var pkDef = new PartitionKeyDefinition
                    {
                        Paths = new Collection<string>() { cosmosDbConnection.PartitionKey },
                    };

                    await documentClient.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(cosmosDbConnection.DatabaseId),
                        new DocumentCollection { Id = cosmosDbConnection.CollectionId, PartitionKey = pkDef },
                        new RequestOptions { OfferThroughput = 1000 }).ConfigureAwait(false);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}