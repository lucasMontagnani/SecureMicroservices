using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Movies.Client.Models;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Movies.Client.ApiServices
{
    public class MovieApiService : IMovieApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MovieApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<IEnumerable<Movie>> GetMovies()
        {
            // The Get Token operation will be in a interceptor with delegate handler to optimize our code

            HttpClient httpClient = _httpClientFactory.CreateClient("MovieAPIClient");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "api/Movies");

            HttpResponseMessage response = await httpClient.SendAsync(request, 
                                                                      HttpCompletionOption.ResponseHeadersRead)
                                                           .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            List<Movie>? movieList = JsonConvert.DeserializeObject<List<Movie>>(content);
            return movieList ?? new List<Movie>();
        }

        public async Task<IEnumerable<Movie>> GetMoviesNonOptimalWay()
        {
            // -- Get token from Identity Server, by providing the IdentityServer Configuration

            // 1. "retrieve" our api credentials. This must be registered on Identity Server!
            ClientCredentialsTokenRequest apiClientCredentials = new()
            {
                Address = "https://localhost:5005/connect/token",

                ClientId = "movieClient",
                ClientSecret = "secret",

                // This is the scope our Protected API requires. 
                Scope = "movieAPI"
            };

            // Creates a new HttpClient to talk to our IdentityServer (localhost:5005)
            HttpClient client = new();

            // Just checks if we can reach the Discovery document. Not 100% needed but..
            DiscoveryDocumentResponse disco = await client.GetDiscoveryDocumentAsync("https://localhost:5005");
            if (disco.IsError) return null; // throw 500 error

            // 2. Authenticates and get an access token from Identity Server
            TokenResponse tokenResponse = await client.RequestClientCredentialsTokenAsync(apiClientCredentials);
            if (tokenResponse.IsError) return null;

            // -- Send Request to the Protected API 

            // Another HttpClient for talking now with our Protected API
            HttpClient apiClient = new();

            // 3. Set the access_token in the request Authorization: Bearer <token>
            client.SetBearerToken(tokenResponse.AccessToken);

            // 4. Send a request to our Protected API
            HttpResponseMessage response = await client.GetAsync("https://localhost:5001/api/movies");
            response.EnsureSuccessStatusCode();

            // Deserialize Object MovieList
            string content = await response.Content.ReadAsStringAsync();

            List<Movie>? movieList = JsonConvert.DeserializeObject<List<Movie>>(content);
            return movieList ?? new List<Movie>();
        }

        public Task<Movie> GetMovie(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Movie> CreateMovie(Movie movie)
        {
            throw new NotImplementedException();
        }

        public Task<Movie> UpdateMovie(Movie movie)
        {
            throw new NotImplementedException();
        }

        public Task DeleteMovie(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<UserInfoViewModel> GetUserInfo()
        {
            HttpClient idpClient = _httpClientFactory.CreateClient("IDPClient");

            DiscoveryDocumentResponse metaDataResponse = await idpClient.GetDiscoveryDocumentAsync();

            if (metaDataResponse.IsError)
                throw new HttpRequestException("Something went wrong while requesting the access token");


            string? accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            UserInfoResponse userInfoResponse = await idpClient.GetUserInfoAsync(
               new UserInfoRequest
               {
                   Address = metaDataResponse.UserInfoEndpoint,
                   Token = accessToken
               });

            if (userInfoResponse.IsError)
                throw new HttpRequestException("Something went wrong while getting user info");

            Dictionary<string, string> userInfoDictionary = new Dictionary<string, string>();

            foreach (Claim claim in userInfoResponse.Claims)
            {
                userInfoDictionary.Add(claim.Type, claim.Value);
            }

            return new UserInfoViewModel(userInfoDictionary);
        }
    }
}
