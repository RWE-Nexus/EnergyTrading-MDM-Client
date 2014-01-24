namespace MDM.Client.PerformanceTests.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    public static class TestExtensions
    {
        public static IEnumerable<string> SplitToList(this string commaSeparatedString)
        {
            if (string.IsNullOrEmpty(commaSeparatedString))
            {
                return Enumerable.Empty<string>();
            }

            return commaSeparatedString
                .Split(',')
                .Select(x => x.Trim());
        }
    }
}
