using System.Security.Claims;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;

namespace AD_Demo.Services;

public class CustomAccountFactory : AccountClaimsPrincipalFactory<CustomUserAccount>
{
    private readonly ILogger<CustomAccountFactory> _logger;
    
    public CustomAccountFactory(IAccessTokenProviderAccessor accessor, ILogger<CustomAccountFactory> logger) : base(accessor)
    {
        _logger = logger;
    }
    
    public async override ValueTask<ClaimsPrincipal> CreateUserAsync(
            CustomUserAccount account,
            RemoteAuthenticationUserOptions options)
        {
            var initialUser = await base.CreateUserAsync(account, options);

            if (initialUser.Identity.IsAuthenticated)
            {
                var userIdentity = (ClaimsIdentity)initialUser.Identity;

                foreach (var role in account.Roles)
                {
                    userIdentity.AddClaim(new Claim("role", role));
                }

                _logger.LogInformation("User {Name} has been successfully created", userIdentity.Name);
            }

            return initialUser;
        }
}