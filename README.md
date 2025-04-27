# SecureMicroservices

This project demonstrates how to secure microservices in a .NET environment using OAuth2, OpenID Connect, and Ocelot API Gateway.

The system is composed of four microservices, working together to authenticate users, authorize API requests, and manage secure communication across services:

- **Movies.Client (MVC Application)**:
An interactive ASP.NET Core MVC client that authenticates users through OpenID Connect via the IdentityServer microservice.
The client sends credentials to IdentityServer, receives a JWT token in response, and uses the bearer token to access secured APIs through the Ocelot API Gateway.

- **Movies.API**:
A secured ASP.NET Core Web API that performs CRUD operations on movie records using Entity Framework Core with an in-memory relational database.
API resources are protected using OAuth2 with JWT tokens issued by IdentityServer.

- **IdentityServer**:
A standalone centralized Authentication Server and Identity Provider implemented using Duende.IdentityServer Library.
It supports OpenID Connect for authentication and OAuth2 for authorization, enabling secure login flows and token-based access control across applications and APIs.

- **Ocelot API Gateway**:
A lightweight API Gateway that acts as a reverse proxy.
After the Movies.Client obtains a JWT token, it sends API requests through Ocelot, which forwards them internally to Movies.API.
Ocelot handles token presentation and validation via IdentityServer during the authorization pipeline, ensuring only authorized requests are processed.
