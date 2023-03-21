using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AD_Demo;
using AD_Demo.Pages;
using AD_Demo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddMsalAuthentication<RemoteAuthenticationState, CustomUserAccount>(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add(builder.Configuration["AzureAd:Scope"]);
    options.UserOptions.RoleClaim = "role";
}).AddAccountClaimsPrincipalFactory<RemoteAuthenticationState, CustomUserAccount, CustomAccountFactory>();

builder.Services.AddAuthorizationCore(config =>
{
    // Define the policies for access control
    // Azure AD provides a group claim for each group the user is a member of
    // each group has a unique ID which is immutable.
    config.AddPolicy("DatabaseAccess", builder =>
        // Check the group claim contains one of the following IDs (Multiple values can be added)
        builder
            .RequireAuthenticatedUser()
            .RequireClaim("role", "user")
    );
    
    config.AddPolicy("TestApplication", builder =>
        builder
            .RequireAuthenticatedUser()
            .RequireClaim("role", "administrator")
    );
});

await builder.Build().RunAsync();