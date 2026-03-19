// ExpressionNode.cs

using System.Collections.Concurrent;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiDevLab;


/// <summary>
/// Represents a single value or expression in the expression graph.
/// The value of an <see cref="ExpressionNode"/> can be set by user input or by evaluating the expression defined by the node.
/// The <see cref="ExpressionManager"/> manages a collection of <see cref="ExpressionNode"/>s and their dependencies,
/// and is responsible for evaluating the expressions and updating the values of the nodes accordingly.
/// </summary>
public partial class ExpressionNode : ObservableObject
{
	/// <summary>
	/// The owning <see cref="ExpressionManager"/> responsible for evaluating
	/// and updating this node.
	/// </summary>
	public ExpressionManager? Owner { get; internal set; }

	/// <summary>
	/// The unique reference string identifying this node within the expression graph.
	/// </summary>
	public string NodeRef { get; internal set; } = string.Empty;

	/// <summary>
	/// Initializes a new instance of the <see cref="ExpressionNode"/> class.
	/// </summary>
	public ExpressionNode()
	{
	}

	/// <summary>
	/// Gets or sets the externally visible value of this node.
	/// </summary>
	public object? Value
	{
		get => InternalValue;
		set => Owner?.SetValue(NodeRef, value, ExpressionValueKind.UserInput);
	}

	/// <summary>
	/// Raises a property-changed notification for <see cref="Value"/>.
	/// </summary>
	public void OnValueChanged()
		=> OnPropertyChanged(nameof(Value));

	/// <summary>
	/// Sets the internal value of the node without triggering dependency propagation.
	/// </summary>
	/// <param name="value">The value to assign.</param>
	internal void SetInternalValue(object? value)
	{
		InternalValue = value;
	}


	/// <summary>
	/// Gets the internally stored value for this node.
	/// </summary>
	internal object? InternalValue { get; private set; }

	/// <summary>
	/// Describes the current lifecycle state of the node.
	/// </summary>
	[ObservableProperty]
	public partial ExpressionValueKind ValueKind { get; internal set; } = ExpressionValueKind.Default;

	/// <summary>
	/// The expected type of the node's value, if constrained.
	/// </summary>
	[ObservableProperty]
	public partial Type? ValueType { get; internal set; } = null;

	/// <summary>
	/// The expression text associated with this node.
	/// </summary>
	[ObservableProperty]
	public partial string Expression { get; internal set; } = string.Empty;

	/// <summary>
	/// The parsed tokens representing the node's expression in evaluation order.
	/// </summary>
	public readonly List<ExpressionToken> Tokens = new();


	/// <summary>
	/// References to nodes that this node depends on as inputs.
	/// </summary>
	public readonly ConcurrentDictionary<string, byte> InputNodeRefs = new();

	/// <summary>
	/// References to nodes that depend on this node as an input.
	/// </summary>
	public readonly ConcurrentDictionary<string, byte> OutputNodeRefs = new();

	/// <summary>
	/// Indicates whether the node's value is deterministic.
	/// </summary>
	[ObservableProperty]
	public partial bool IsDeterministic { get; internal set; } = true;


	/// <summary>
	/// Returns a string representation of the node's current value.
	/// </summary>
	public override string? ToString() => InternalValue?.ToString();

	/// <summary>
	/// Resets the node to an uninitialized state and clears all internal data.
	/// </summary>
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
