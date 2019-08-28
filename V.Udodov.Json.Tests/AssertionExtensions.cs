using Newtonsoft.Json.Linq;
using Xunit;

namespace V.Udodov.Json.Tests
{
    public static class AssertionExtensions
    {
        public static void JsonEquals(this string actual, string expected)
        {
            var o1 = JObject.Parse(actual);
            var o2 = JObject.Parse(expected);

            Assert.True(JToken.DeepEquals(o1, o2), "JSON are not deeply equal");
        }
    }
}