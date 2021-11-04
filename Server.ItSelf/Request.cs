using System.Net.Http;

namespace Server.ItSelf
{
    public record Request(string Path, HttpMethod Method);
}
