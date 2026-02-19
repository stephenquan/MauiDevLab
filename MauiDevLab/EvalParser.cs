// EvalParser.cs

using System.Text.RegularExpressions;

namespace MauiDevLab;

public enum EvalTokenType
{
	Value,
	Binding,
	Operator,
}

public sealed record EvalToken(EvalTokenType Type, string Text, object? Value = null, Func<object?[], object?>? Func = null);

public partial class EvalParser
{
	bool valid = false;
	public bool IsValid => valid;

	string expression = string.Empty;
	int index = 0;
	List<EvalToken> tokens = [];
	Match match = Match.Empty;

	static object? Zero(object?[] args) => 0;

	static Dictionary<string, Func<object?[], object?>> operatorDict
		= new()
		{
			["*"] = Mul,
			["/"] = Div,
			["+"] = Add,
			["-"] = Sub,
			["mod"] = Mod
		};

	static object? Mul(object?[] args) => WrapFunc<double, double, double>((a, b) => a * b, args);
	static object? Add(object?[] args) => WrapFunc<double, double, double>((a, b) => a + b, args);
	static object? Sub(object?[] args) => WrapFunc<double, double, double>((a, b) => a - b, args);
	static object? Div(object?[] args) => WrapFunc<double, double, double>((a, b) => a / b, args);
	static object? Mod(object?[] args) => WrapFunc<double, double, double>((a, b) => a % b, args);

	static bool TryConvert<T>(object? value, out T valueT)
	{
		valueT = default!;

		if (value is null)
		{
			return false;
		}

		try
		{
			if (typeof(T) == typeof(double))
			{
				valueT = (T)(object)Convert.ToDouble(value);
			}
			return true;
		}
		catch
		{
			return false;
		}
	}

	static object? WrapFunc<T1, T2, TReturn>(Func<T1, T2, TReturn> func, object?[] args)
		=> (args.Length == 2
			&& TryConvert<T1>(args[0], out T1 p1)
			&& TryConvert<T2>(args[1], out T2 p2))
			? func(p1, p2)
			: null;

	/*
	static object? AddOperator(object?[] args)
		=> WrapMathFunc((a, b) => a + b)(args);

	static object? AddOperator(object?[] args)
		=> (args.Length == 2
			&& TryConvertToDouble(args[0], out var dblA)
			&& TryConvertToDouble(args[1], out var dblB))
			? dblA + dblB
			: null;
	*/

	public EvalParser(string expression)
	{
		this.expression = expression;
		index = 0;
		valid = Parse() && index == expression.Length;
	}

	bool Parse()
	{
		return ParseSum();
	}

	[GeneratedRegex("""^([+-])""")]
	private static partial Regex SumRegex();
	bool ParseSum() => ParseBinaryOperators(SumRegex(), ParseProduct);

	[GeneratedRegex("""^(\*|mod|\/])""")]
	private static partial Regex ProductRegex();
	bool ParseProduct() => ParseBinaryOperators(ProductRegex(), ParsePrimary);

	[GeneratedRegex("""^(\-?\d+\.\d+|\-?\d+)""")]
	private static partial Regex NumberRegex();
	[GeneratedRegex("""^{([^}]*)}""")]
	private static partial Regex BindingRegex();
	[GeneratedRegex("""^(\()""")]
	private static partial Regex ParenStartRegex();
	[GeneratedRegex("""^(\))""")]
	private static partial Regex ParenEndRegex();
	[GeneratedRegex("""^[`]([^`]*)[`]""")]
	private static partial Regex BacktickStringRegex();
	bool ParsePrimary()
	{
		if (ParsePattern(NumberRegex()))
		{
			string numberText = match.Groups[1].Value;
			if (int.TryParse(numberText, out var intValue))
			{
				tokens.Add(new EvalToken(EvalTokenType.Value, numberText, intValue));
				return true;
			}
			if (double.TryParse(numberText, out var dblValue))
			{
				tokens.Add(new EvalToken(EvalTokenType.Value, numberText, dblValue));
				return true;
			}
			return false;
		}
		if (ParsePattern(BacktickStringRegex()))
		{
			tokens.Add(new EvalToken(EvalTokenType.Value, match.Groups[0].Value, match.Groups[1].Value));
			return true;
		}

		if (ParsePattern(BindingRegex()))
		{
			tokens.Add(new EvalToken(EvalTokenType.Binding, match.Groups[1].Value, null));
			return true;
		}

		int _index = index;
		if (ParsePattern(ParenStartRegex()))
		{
			if (!Parse())
			{
				index = _index;
				return false;
			}
			if (!ParsePattern(ParenEndRegex()))
			{
				index = _index;
				return false;
			}
			return true;
		}
		return false;
	}

	bool ParseBinaryOperators(Regex BinaryOperators, Func<bool> ParseNext)
	{
		if (!ParseNext())
		{
			return false;
		}
		int _index = index;
		while (ParsePattern(BinaryOperators))
		{
			string _operator = match.Groups[1].Value;
			if (!ParseNext())
			{
				this.index = _index;
				return false;
			}
			tokens.Add(new EvalToken(EvalTokenType.Operator, _operator, null, operatorDict[_operator]));
			_index = index;
		}
		return true;
	}

	[GeneratedRegex("""^\s*""")]
	private static partial Regex WhitespaceRegex();

	bool ParsePattern(Regex regex)
	{
		var whitespaceMatch = WhitespaceRegex().Match(expression[index..]);
		if (whitespaceMatch.Success)
		{
			index += whitespaceMatch.Length;
		}
		match = regex.Match(expression[index..]);
		if (!match.Success)
		{
			return false;
		}
		index += match.Length;
		whitespaceMatch = WhitespaceRegex().Match(expression[index..]);
		if (whitespaceMatch.Success)
		{
			index += whitespaceMatch.Length;
		}
		return true;
	}

	public BindingBase CreateBinding()
	{
		if (tokens.Count > 0 && tokens[0].Type == EvalTokenType.Binding)
		{
			return new Binding(tokens[0].Text);
		}

		return new Binding(".");
	}
}
