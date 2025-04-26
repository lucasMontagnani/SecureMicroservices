using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Movies.Client.HttpHandlers
{
    public class AuthenticationDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly ClientCredentialsTokenRequest _tokenRequest;

        public AuthenticationDelegatingHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /*
         * This class will intercept all the HTTP requests and try to get the Token before sending request to the protected API.
         */

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //var httpClient = _httpClientFactory.CreateClient("IDPClient");

            //var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(_tokenRequest);
            //if (tokenResponse.IsError)
            //{
            //    throw new HttpRequestException("Something went wrong while requesting the access token");
            //}
            //request.SetBearerToken(tokenResponse.AccessToken);

            string? accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            if (!string.IsNullOrWhiteSpace(accessToken))
                request.SetBearerToken(accessToken);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
