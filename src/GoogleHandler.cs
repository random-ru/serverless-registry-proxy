namespace rpnpm;

using System.Net.Http.Headers;
using System.Text;

public class GoogleHandler : DelegatingHandler
{
    private string UserName { get; set; }
    private string Password { get; set; }
    private string BasisAuth { get; set; }
    public GoogleHandler(IConfiguration config)
    {
        var block = config.GetSection("GoogleSSE");
        UserName = block["username"];
        Password = block["password"];
        BasisAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{UserName}:{Password}"));
    }
    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", BasisAuth);
        return base.Send(request, cancellationToken);
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", BasisAuth);
        return base.SendAsync(request, cancellationToken);
    }
}