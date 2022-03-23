using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AspnetRunBasics.Extensions;
using AspnetRunBasics.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AspnetRunBasics.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _client;
        private readonly ILogger<CatalogService> _logger;
        private readonly IConfiguration _configuration;

        public CatalogService(HttpClient client, ILogger<CatalogService> logger, IConfiguration configuration)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<IEnumerable<CatalogModel>> GetCatalog()
        {
            _logger.LogError($"Configuration found: {_configuration.GetConnectionString("IdentityAuthority")}");
            var response = await _client.GetAsync("/Catalog");
            return await response.ReadContentAs<List<CatalogModel>>();
        }

        public async Task<CatalogModel> GetCatalog(string id)
        {
            var response = await _client.GetAsync($"/Catalog/{id}");
            return await response.ReadContentAs<CatalogModel>();
        }

        public async Task<IEnumerable<CatalogModel>> GetCatalogByCategory(string category)
        {
            var response = await _client.GetAsync($"/Catalog/GetProductsByCategory/{category}");
            return await response.ReadContentAs<List<CatalogModel>>();
        }

        public async Task<CatalogModel> CreateCatalog(CatalogModel model)
        {
            var response = await _client.PostAsJson($"/Catalog", model);
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<CatalogModel>();
            else
            {
                throw new Exception("Something went wrong when calling api.");
            }
        }
    }
}