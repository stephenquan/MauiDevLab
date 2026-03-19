// ExpressionNode.cs

using System.Collections.Concurrent;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiDevLab;

public partial class ExpressionNode : ObservableObject
{
	public ExpressionManager? Owner { get; internal set; }
	public string NodeRef { get; internal set; } = string.Empty;

	public ExpressionNode()
	{
	}

	public object? Value
	{
		get => InternalValue;
		set => Owner?.SetValue(NodeRef, value, ExpressionValueKind.UserInput);
	}

	public void OnValueChanged()
		=> OnPropertyChanged(nameof(Value));


	internal void SetInternalValue(object? value)
	{
		InternalValue = value;
	}

	internal object? InternalValue { get; private set; }

	[ObservableProperty]
	public partial ExpressionValueKind ValueKind { get; set; } = ExpressionValueKind.Default;

	[ObservableProperty]
	public partial Type? ValueType { get; internal set; } = null;

	[ObservableProperty]
	public partial string Expression { get; internal set; } = string.Empty;

	public readonly List<ExpressionToken> Tokens = new();
	public readonly ConcurrentDictionary<string, byte> InputNodeRefs = new();
	public readonly ConcurrentDictionary<string, byte> OutputNodeRefs = new();

	[ObservableProperty]
	public partial bool IsDeterministic { get; internal set; } = true;

	public override string? ToString() => InternalValue?.ToString();

	internal void Clear()
	{
		Owner = null;
		InternalValue = null;
		ValueKind = ExpressionValueKind.Uninitialized;
		ValueType = null;
		Expression = string.Empty;
		InputNodeRefs.Clear();
		OutputNodeRefs.Clear();
		Tokens.Clear();
	}
}
