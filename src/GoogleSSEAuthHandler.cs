namespace rpnpm;

using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

public class GoogleSSEAuthHandler : AuthenticationHandler<GoogleSSEAuthOptions>
{
    private readonly IConfiguration _config;
    private readonly List<string> _tokenList;

    public GoogleSSEAuthHandler(IOptionsMonitor<GoogleSSEAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IConfiguration config)
        : base(options, logger, encoder, clock)
    {
        _config = config;
        _tokenList = _config.GetSection("TokenList").Get<List<string>>();
    }
    private const string AuthorizationHeaderName = "Authorization";
    private const string BasicSchemeName = "Bearer";
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!AuthenticationHeaderValue.TryParse(Request.Headers[AuthorizationHeaderName], out var headerValue))
        {
            //Invalid Authorization header
            return Task.FromResult(AuthenticateResult.Fail("Cannot read authorization header."));
        }
        if (!BasicSchemeName.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(AuthenticateResult.Fail("Cannot read authorization sehema."));

        if (!_tokenList.Contains(headerValue.Parameter!))
            return Task.FromResult(AuthenticateResult.Fail("Access denied."));

        var identities = new List<ClaimsIdentity> { new ClaimsIdentity("token") };
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identities), Options.Scheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

public class GoogleSSEAuthOptions : AuthenticationSchemeOptions
{
    public const string CustomAuth = "GoogleSSE";
    public string Scheme => CustomAuth;
}
