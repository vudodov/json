using Xunit;

namespace V.Udodov.Json.Tests
{
    public static class AssertionExtensions
    {
        public static void JsonEquals(this string actual, string expected)
        {
            int actualIndex = 0, expectedIndex = 0;

            while (actualIndex < actual.Length && expectedIndex < expected.Length)
            {
                while (actualIndex < actual.Length && !IsValidChar(actual[actualIndex])) actualIndex++;
                while (expectedIndex < expected.Length && !IsValidChar(expected[expectedIndex])) expectedIndex++;

                Assert.True(
                    actualIndex < actual.Length && expectedIndex < expected.Length,
                    "JSON should have same length");
                Assert.True(
                    actual[actualIndex] == expected[expectedIndex],
                    $"JSON not matching from {actual.Substring(actualIndex)} and {expected.Substring(expectedIndex)}");

                actualIndex++;
                expectedIndex++;
            }

            Assert.True(
                actualIndex == actual.Length && expectedIndex == expected.Length,
                "JSON should have same length");

            bool IsValidChar(char c) =>
                c >= '0' && c <= '9' ||
                c >= 'A' && c <= 'Z' ||
                c >= 'a' && c <= 'z' ||
                c == '{' || c == '}' ||
                c == '_' || c == '\'' ||
                c == '\"' || c == '$' ||
                c == '+' || c == '-' ||
                c == ',' || c == '.' ||
                c == ']' || c == '[' ||
                c == ':';
        }
    }
}