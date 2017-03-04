using System;
using System.Text.RegularExpressions;

namespace MetroFramework.Drawing.Html
{
	internal static class Parser
	{
		public const string CssProperties = ";?[^;\\s]*:[^\\{\\}:;]*(\\}|;)?";

		public const string CssComments = "/\\*[^*/]*\\*/";

		public const string CssAtRules = "@.*\\{\\s*(\\s*[^\\{\\}]*\\{[^\\{\\}]*\\}\\s*)*\\s*\\}";

		public const string CssMediaTypes = "@media[^\\{\\}]*\\{";

		public const string CssBlocks = "[^\\{\\}]*\\{[^\\{\\}]*\\}";

		public const string CssNumber = "{[0-9]+|[0-9]*\\.[0-9]+}";

		public const string CssPercentage = "([0-9]+|[0-9]*\\.[0-9]+)\\%";

		public const string CssLength = "([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)";

		public const string CssColors = "(#\\S{6}|#\\S{3}|rgb\\(\\s*[0-9]{1,3}\\%?\\s*\\,\\s*[0-9]{1,3}\\%?\\s*\\,\\s*[0-9]{1,3}\\%?\\s*\\)|maroon|red|orange|yellow|olive|purple|fuchsia|white|lime|green|navy|blue|aqua|teal|black|silver|gray)";

		public const string CssLineHeight = "(normal|{[0-9]+|[0-9]*\\.[0-9]+}|([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)|([0-9]+|[0-9]*\\.[0-9]+)\\%)";

		public const string CssBorderStyle = "(none|hidden|dotted|dashed|solid|double|groove|ridge|inset|outset)";

		public const string CssBorderWidth = "(([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)|thin|medium|thick)";

		public const string CssFontFamily = "(\"[^\"]*\"|'[^']*'|\\S+\\s*)(\\s*\\,\\s*(\"[^\"]*\"|'[^']*'|\\S+))*";

		public const string CssFontStyle = "(normal|italic|oblique)";

		public const string CssFontVariant = "(normal|small-caps)";

		public const string CssFontWeight = "(normal|bold|bolder|lighter|100|200|300|400|500|600|700|800|900)";

		public const string CssFontSize = "(([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)|([0-9]+|[0-9]*\\.[0-9]+)\\%|xx-small|x-small|small|medium|large|x-large|xx-large|larger|smaller)";

		public const string CssFontSizeAndLineHeight = "(([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)|([0-9]+|[0-9]*\\.[0-9]+)\\%|xx-small|x-small|small|medium|large|x-large|xx-large|larger|smaller)(\\/(normal|{[0-9]+|[0-9]*\\.[0-9]+}|([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)|([0-9]+|[0-9]*\\.[0-9]+)\\%))?(\\s|$)";

		public const string HtmlTag = "<[^<>]*>";

		public const string HmlTagAttributes = "[^\\s]*\\s*=\\s*(\"[^\"]*\"|[^\\s]*)";

		public static MatchCollection Match(string regex, string source)
		{
			return (new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.Singleline)).Matches(source);
		}

		public static string Search(string regex, string source)
		{
			int num;
			return Parser.Search(regex, source, out num);
		}

		public static string Search(string regex, string source, out int position)
		{
			MatchCollection matchCollections = Parser.Match(regex, source);
			if (matchCollections.Count <= 0)
			{
				position = -1;
				return null;
			}
			position = matchCollections[0].Index;
			return matchCollections[0].Value;
		}
	}
}