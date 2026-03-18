// ExpressionParserDefaults.cs

using System.Text.RegularExpressions;

namespace MauiDevLab;

public static partial class ExpressionParserDefaults
{
	[GeneratedRegex(@"\s*(or)\s*")]
	public static partial Regex LogicalOrOperatorRegex();

	[GeneratedRegex(@"\s*(and)\s*")]
	public static partial Regex LogicalAndOperatorRegex();

	[GeneratedRegex(@"\s*(=|!=)\s*")]
	public static partial Regex EqualityOperatorsRegex();

	[GeneratedRegex(@"\s*(<=|<|>=|>)\s*")]
	public static partial Regex ComparisonOperatorsRegex();

	[GeneratedRegex(@"\s*(\+|\-)\s*")]
	public static partial Regex SumRegex();

	[GeneratedRegex(@"\s*(\*|div|mod)\s*")]
	public static partial Regex ProductRegex();

	[GeneratedRegex(@"((?:\/[A-Za-z_][A-Za-z0-9_-]*)+)")]
	public static partial Regex NodeAbsoluteRegex();

	[GeneratedRegex(@"\s*(-?(?:\d+\.\d+|\d+))\s*")]
	public static partial Regex NumberRegex();

	[GeneratedRegex(@"\s*([A-Za-z_][A-Za-z0-9_:-]*)\s*")]
	public static partial Regex IdentifierRegex();

	[GeneratedRegex(@"\s*(\()\s*")]
	public static partial Regex FunctionStartRegex();

	[GeneratedRegex(@"\s*(\))\s*")]
	public static partial Regex FunctionEndRegex();

	[GeneratedRegex(@"\s*(,)\s*")]
	public static partial Regex FunctionCommaRegex();

	[GeneratedRegex(@"\s*(\()\s*")]
	public static partial Regex ParenStartRegex();

	[GeneratedRegex(@"\s*(\))\s*")]
	public static partial Regex ParenEndRegex();

	[GeneratedRegex(@"\s*(-)\s*")]
	public static partial Regex NegateRegex();
}
