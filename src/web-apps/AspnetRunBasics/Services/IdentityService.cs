using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace AspnetRunBasics.Services
{
	public class IdentityService : IIdentityService
	{
        private readonly HttpClient _client;
        private readonly ClientCredentialsTokenRequest _tokenRequest;

        public IdentityService(HttpClient client, ClientCredentialsTokenRequest tokenRequest)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _tokenRequest = tokenRequest ?? throw new ArgumentNullException(nameof(tokenRequest));
        }

        public async Task<TokenResponse> RequestClientCredentialsTokenAsync()
        {
            var response = await _client.RequestClientCredentialsTokenAsync(_tokenRequest);

            if (response.IsError)
                throw new HttpRequestException(response.Error);

            return response;
        }
    }
}

