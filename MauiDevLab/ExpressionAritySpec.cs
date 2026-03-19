// ExpressionAritySpec.cs

namespace MauiDevLab;


public readonly record struct AritySpec
{
	public int Min { get; }
	public int? Max { get; }

	public AritySpec(int min, int? max)
	{
		if (min < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(min));
		}
		if (max is not null && max < min)
		{
			throw new ArgumentOutOfRangeException(nameof(max));
		}

		Min = min;
		Max = max;
	}

	public static AritySpec Exactly(int n) => new(n, n);
	public static AritySpec Range(int min, int max) => new(min, max);
	public static AritySpec AtLeast(int min) => new(min, null);

	public static AritySpec Zero => Exactly(0);
	public static AritySpec ZeroOrMore => AtLeast(0);
	public static AritySpec One => Exactly(1);
	public static AritySpec OneOrTwo => Range(1, 2);
	public static AritySpec OneOrMore => AtLeast(1);
	public static AritySpec Two => Exactly(2);
	public static AritySpec TwoOrThree => Range(2, 3);
	public static AritySpec Three => Exactly(3);

	public bool IsExactly(int n) => Min == n && Max == n;
	public bool IsZero => IsExactly(0);
	public bool IsOne => IsExactly(1);
	public bool IsTwo => IsExactly(2);

	public bool IsFixed => Max is int max && max == Min;
	public bool IsRange => Max is int max && max != Min;
	public bool IsInfinite => Max is null;

	public bool Accepts(int arity) =>
		arity >= Min && (Max is null || arity <= Max.Value);

	public override string ToString() =>
		Max is null ? $"{Min}..Infinity" : (Min == Max ? Min.ToString() : $"{Min}..{Max}");
}
