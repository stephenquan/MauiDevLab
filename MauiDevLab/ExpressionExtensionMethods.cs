// ExpressionExtensionMethods.cs

using System.Globalization;
using Microsoft.Extensions.Logging;

namespace MauiDevLab;

public static class ExpressionExtensionMethods
{
	public static bool TryConvert<T>(this object? value, out T coerceValue)
	{
		if (TryConvert(value, typeof(T), out var _coerceValue) && _coerceValue is T coerce)
		{
			coerceValue = coerce;
			return true;
		}
		coerceValue = default!;
		return false;
	}

	public static bool TryConvert(this object? value, Type? valueType, out object? coerceValue)
	{
		coerceValue = value;
		if (value is null || valueType is null)
		{
			return true;
		}
		try
		{
			if (valueType == typeof(bool))
			{
				coerceValue = Convert.ToBoolean(value, CultureInfo.InvariantCulture);
				return true;
			}
			if (valueType == typeof(double))
			{
				coerceValue = Convert.ToDouble(value, CultureInfo.InvariantCulture);
				return true;
			}
			if (valueType == typeof(long))
			{
				coerceValue = Convert.ToInt64(value, CultureInfo.InvariantCulture);
				return true;
			}
			if (valueType == typeof(int))
			{
				coerceValue = Convert.ToInt32(value, CultureInfo.InvariantCulture);
				return true;
			}
			if (valueType == typeof(string))
			{
				coerceValue = Convert.ToString(value, CultureInfo.InvariantCulture);
				return true;
			}
			if (valueType == typeof(object))
			{
				return true;
			}
			return false;
		}
		catch
		{
			ExpressionManager.Logger?.LogWarning("Failed to convert value '{Value}' of type {ValueType} to {TargetType}", value, value.GetType(), valueType);
			return false;
		}
	}

	public static TReturn? WrapFunc<T1, TReturn>(this object?[] args, Func<T1, TReturn> func)
		=> (args.Length >= 1
			&& args[0].TryConvert<T1>(out var p1))
		? func(p1)
		: default;

	public static TReturn? WrapFunc<T1, T2, TReturn>(this object?[] args, Func<T1, T2, TReturn> func)
		=> (args.Length >= 2
			&& args[0].TryConvert<T1>(out var p1)
			&& args[1].TryConvert<T2>(out var p2))
		? func(p1, p2)
		: default;

	public static TReturn? WrapFunc<T1, T2, T3, TReturn>(this object?[] args, Func<T1, T2, T3, TReturn> func)
		=> (args.Length >= 3
			&& args[0].TryConvert<T1>(out var p1)
			&& args[1].TryConvert<T2>(out var p2)
			&& args[2].TryConvert<T3>(out var p3))
		? func(p1, p2, p3)
		: default;


	public static bool Initialize(this ExpressionNode node, ExpressionParserPlugin plugin)
	{
		ExpressionParser parser = new(plugin);

		if (!parser.TryParse(node.NodeRef, node.Expression))
		{
			node.ValueKind = ExpressionValueKind.ParseError;
			return false;
		}

		node.Tokens.Clear();

		foreach (var token in parser.Tokens)
		{
			node.Tokens.Add(token);
			if (token.TokenType == ExpressionTokenType.Node)
			{
				node.InputNodeRefs.TryAdd(token.Text, 0);
			}
		}

		node.ValueKind = ExpressionValueKind.PendingCalculation;
		return true;
	}

	public static bool Calculate(this ExpressionNode node, Func<string, object?> getValue, CancellationToken ct)
	{
		switch (node.ValueKind)
		{
			case ExpressionValueKind.Default:
			case ExpressionValueKind.Folder:
			case ExpressionValueKind.UserInput:
			case ExpressionValueKind.Calculated:
			case ExpressionValueKind.PendingCalculation:
				node.ValueKind = ExpressionValueKind.PendingCalculation;
				break;
			case ExpressionValueKind.Uninitialized:
			case ExpressionValueKind.ParseError:
			case ExpressionValueKind.CalculateError:
			default:
				return false;
		}

		bool isDeterministic = true;
		Stack<object?> rpn = [];
		for (int i = 0; !ct.IsCancellationRequested && node.ValueKind == ExpressionValueKind.PendingCalculation && i < node.Tokens.Count; i++)
		{
			var token = node.Tokens[i];
			switch (token.TokenType)
			{
				case ExpressionTokenType.Constant:
					rpn.Push(token.Value);
					break;
				case ExpressionTokenType.Node:
					rpn.Push(getValue(token.Text));
					break;
				case ExpressionTokenType.Operator:
				case ExpressionTokenType.Function:
					ArgumentNullException.ThrowIfNull(token.FunctionInfo);
					if (rpn.Count < token.FunctionArity)
					{
						node.ValueKind = ExpressionValueKind.CalculateError;
						return false;
					}
					var args = new object?[token.FunctionArity];
					for (int j = token.FunctionArity - 1; j >= 0; j--)
					{
						args[j] = rpn.Pop();
					}
					rpn.Push(token.FunctionInfo.Function(args));
					if (token.FunctionInfo.IsDeterministic == false)
					{
						isDeterministic = false;
					}
					break;
				default:
					node.ValueKind = ExpressionValueKind.CalculateError;
					return false;
			}
		}

		if (rpn.Count != 1)
		{
			node.ValueKind = ExpressionValueKind.CalculateError;
			return false;
		}

		object? value = rpn.Pop();

		if (value is null && node.InternalValue is null)
		{
			node.ValueKind = ExpressionValueKind.Calculated;
			node.IsDeterministic = isDeterministic;
			return false;
		}

		if (value is not null && node.InternalValue is not null && value.Equals(node.InternalValue))
		{
			node.ValueKind = ExpressionValueKind.Calculated;
			node.IsDeterministic = isDeterministic;
			return false;
		}

		node.SetInternalValue(value);
		node.ValueKind = ExpressionValueKind.Calculated;
		node.IsDeterministic = isDeterministic;
		ExpressionManager.Logger?.LogTrace("Calculated {NodeRef} to {Value} (type={ValueType}, valueKind={ValueKind}, isDeterministic={IsDeterministic})", node.NodeRef, value, node.ValueType, node.ValueKind, node.IsDeterministic);
		return true;
	}
}
