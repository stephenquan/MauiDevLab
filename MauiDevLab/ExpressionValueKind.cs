// ExpressionValueKind.cs

namespace MauiDevLab;

public enum ExpressionValueKind
{
	Uninitialized,
	Default,
	Folder,
	UserInput,
	PendingCalculation,
	Calculated,

	ParseError,
	CalculateError,
}

