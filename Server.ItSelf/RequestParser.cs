using System.Net.Http;

namespace Server.ItSelf
{
    internal static class RequestParser
    {
        public static Request Parse(string header)
        {
            var split = header.Split(" ");
            return new Request(split[1], GetMethod(split[0]));
        }

        private static HttpMethod GetMethod(string method)
        {
            if (method == "GET")
                return HttpMethod.Get;
            return HttpMethod.Post;
        }
    }
}
