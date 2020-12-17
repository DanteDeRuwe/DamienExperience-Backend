using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace DamianTourBackend.Tests.IntegrationTests.Helpers
{
    public class JsonContent : StringContent
    {
        public JsonContent(object value) : base(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json") { }
    }
}