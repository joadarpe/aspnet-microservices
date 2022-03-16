using System.Threading.Tasks;
using IdentityModel.Client;

namespace AspnetRunBasics.Services
{
    public interface IIdentityService
	{
		Task<TokenResponse> RequestClientCredentialsTokenAsync();
	}
}

