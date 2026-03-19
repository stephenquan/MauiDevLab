// ExpressionFunctionInfo.cs

namespace MauiDevLab;

public record ExpressionFunctionInfo(
	Func<object?[], object?> Function,
	AritySpec AritySpec,
	bool IsDeterministic = true);
