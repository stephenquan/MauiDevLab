// ExpressionToken.cs

namespace MauiDevLab;

public record ExpressionToken(
	ExpressionTokenType TokenType,
	string Text,
	object? Value = null,
	ExpressionFunctionInfo? FunctionInfo = null,
	int FunctionArity = 0);
