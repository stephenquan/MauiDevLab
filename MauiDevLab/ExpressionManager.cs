// ExpressionManager.cs

using System.Collections.Concurrent;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace MauiDevLab;

public partial class ExpressionManager : INotifyPropertyChanged, IDisposable
{
	public static ILogger? Logger { get; } = IPlatformApplication.Current?.Services.GetService<ILogger<ExpressionManager>>();

	public readonly ExpressionParserPlugin ParserPlugin = new();

	readonly ConcurrentDictionary<string, ExpressionNode> dictionary = new();
	readonly BlockingCollection<ExpressionNode> pendingCalculations = new();
	readonly ConcurrentDictionary<ExpressionNode, byte> queuedNodes = new();
	readonly WeakEventManager propertyChangedEventManager = new();
	static readonly ExpressionNode quitNode = new();
	volatile bool isRunning = false;
	Task? runningTask;

	public event PropertyChangedEventHandler? PropertyChanged
	{
		add => propertyChangedEventManager.AddEventHandler(value);
		remove => propertyChangedEventManager.RemoveEventHandler(value);
	}

	Action<Action>? invokeOnUIThread;

	public void SetInvokeOnUIThread(Action<Action> invokeOnUIThread)
	{
		this.invokeOnUIThread = invokeOnUIThread;
	}

	public void SetInvokeOnUIThread(IDispatcher dispatcher)
	{
		this.invokeOnUIThread = action => dispatcher.Dispatch(action);
	}

	public void InvokeOnUIThread(Action action)
	{
		if (invokeOnUIThread is not null)
		{
			invokeOnUIThread(action);
		}
		else
		{
			action();
		}
	}

	public object? this[string nodeRef]
	{
		get => GetOrAddNode(nodeRef).InternalValue;
		set => SetValue(nodeRef, value, ExpressionValueKind.UserInput);
	}

	public T GetValue<T>(string nodeRef)
		=> (GetValue(nodeRef) is T value) ? value : default!;

	public object? GetValue(string nodeRef)
		=> TryGetValueInfo(nodeRef, out var valueInfo) ? valueInfo?.InternalValue : null;

	public bool TryGetValueInfo(string nodeRef, out ExpressionNode? valueInfo)
		=> dictionary.TryGetValue(nodeRef, out valueInfo);

	public ExpressionNode GetOrAddNode(string nodeRef, Type? valueType = null, string? expression = null)
		=> dictionary.GetOrAdd(nodeRef, nodeRef => new ExpressionNode
		{
			Owner = this,
			NodeRef = nodeRef,
			ValueKind = ExpressionValueKind.Uninitialized,
			Expression = expression ?? string.Empty,
			ValueType = valueType,
			IsDeterministic = false
		});

	public ExpressionNode SetValue<T>(string nodeRef, object? value, ExpressionValueKind reason = ExpressionValueKind.Default)
		=> SetValue(nodeRef, value, reason, typeof(T?));

	public ExpressionNode SetValue(string nodeRef, object? value, ExpressionValueKind valueKind = ExpressionValueKind.Default, Type? valueType = default, bool isDeterministic = true, bool trace = true)
	{
		bool modified = false;
		var node = dictionary.GetOrAdd(nodeRef, nodeRef =>
		{
			modified = true;
			return new ExpressionNode
			{
				Owner = this,
				NodeRef = nodeRef,
				ValueKind = valueKind,
				ValueType = valueType,
				IsDeterministic = isDeterministic,
			};
		});
		node.ValueKind = valueKind;
		node.IsDeterministic = isDeterministic;
		RemoveDependencies(node);
		if (valueType is not null && node.ValueType != valueType)
		{
			node.ValueType = valueType;
			modified = true;
		}
		if (value.TryConvert(node.ValueType, out var coerceValue))
		{
			value = coerceValue;
		}
		if (!modified && value is null && node.InternalValue is null)
		{
			node.ValueKind = valueKind;
			node.ValueType = valueType;
			return node;
		}
		if (!modified && value is not null && value.Equals(node.InternalValue))
		{
			node.ValueKind = valueKind;
			node.ValueType = valueType;
			return node;
		}

		Logger?.LogTrace("SetValue {NodeRef} to {Value} (type={ValueType}, valueKind={ValueKind}, isDeterministic={IsDeterministic})", nodeRef, value, valueType, valueKind, isDeterministic);

		node.SetInternalValue(value);
		node.ValueKind = valueKind;
		node.ValueType = valueType;

		NotifyValueChanged(nodeRef);

		return node;
	}

	public void NotifyValueChanged(string nodeRef)
	{
		//InvokeOnUIThread(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs($"Item[{nodeRef}]")));

		if (dictionary.TryGetValue(nodeRef, out var _node) && _node is ExpressionNode node)
		{
			InvokeOnUIThread(() =>
			{
				propertyChangedEventManager.HandleEvent(this, new PropertyChangedEventArgs($"Item[{nodeRef}]"), nameof(PropertyChanged));
				node.OnValueChanged();
			});

			foreach (var outputKey in node.OutputNodeRefs.Keys)
			{
				RecalculateByNodeRef(outputKey);
			}
		}
	}

	public ExpressionNode SetExpression<T>(string nodeRef, string expression)
		=> SetExpression(nodeRef, expression, typeof(T?));

	public ExpressionNode SetExpression(string nodeRef, string expression, Type? valueType = default)
	{
		var node = GetOrAddNode(nodeRef);
		node.ValueKind = ExpressionValueKind.Uninitialized;
		node.ValueType = valueType;
		node.Expression = expression;

		RemoveDependencies(node);

		if (queuedNodes.TryAdd(node, 0))
		{
			pendingCalculations.Add(node);
			Logger?.LogTrace("SetExpression {NodeRef} to {Expression} (type={ValueType})", nodeRef, expression, valueType);
		}

		return node;
	}

	[RelayCommand]
	public void RecalculateByNodeRef(string nodeRef)
	{
		if (dictionary.TryGetValue(nodeRef, out var node))
		{
			RecalculateNode(node);
		}
	}

	public void RecalculateNode(ExpressionNode node)
	{
		if (queuedNodes.TryAdd(node, 0))
		{
			pendingCalculations.Add(node);
		}
	}

	public void StartCalculationLoop(CancellationToken ct)
	{
		if (isRunning)
		{
			return;
		}

		isRunning = true;

		runningTask = Task.Run(async () =>
		{
			Logger?.LogTrace("Starting calculations");
			try
			{
				while (isRunning && !ct.IsCancellationRequested)
				{
					try
					{
						var node = pendingCalculations.Take(ct);
						queuedNodes.TryRemove(node, out _);

						if (node == quitNode)
						{
							break;
						}

						switch (node.ValueKind)
						{
							case ExpressionValueKind.Uninitialized:
								if (!node.Initialize(ParserPlugin))
								{
									Logger?.LogError("Failed to initialize {NodeRef}'s expression {Expression}", node.NodeRef, node.Expression);
									continue;
								}
								UpdateDependencyGraph(node);
								if (node.Calculate(GetValue, ct))
								{
									NotifyValueChanged(node.NodeRef);
								}
								break;
							case ExpressionValueKind.Calculated:
							case ExpressionValueKind.PendingCalculation:
								if (node.Calculate(GetValue, ct))
								{
									NotifyValueChanged(node.NodeRef);
								}
								break;
							case ExpressionValueKind.ParseError:
							case ExpressionValueKind.CalculateError:
							default:
								// Do nothing, won't be re-enqueued
								break;
						}
					}
					catch (OperationCanceledException)
					{
						break;
					}
				}
			}
			finally
			{
				Logger?.LogTrace("Stopping calculations");
				isRunning = false;
				runningTask = null;
			}
		}, ct);
	}

	public async Task StopCalculationLoop()
	{
		if (isRunning)
		{
			isRunning = false;
			pendingCalculations.Add(quitNode);
			if (runningTask is not null)
			{
				await runningTask;
			}
		}
	}

	void UpdateDependencyGraph(ExpressionNode node)
	{
		foreach (var inputNodeRefs in node.InputNodeRefs.Keys)
		{
			var inputNode = dictionary.GetOrAdd(inputNodeRefs, nodeRef => new ExpressionNode
			{
				Owner = this,
				NodeRef = nodeRef,
				ValueKind = ExpressionValueKind.Uninitialized,
				ValueType = null,
				IsDeterministic = false
			});
			inputNode.OutputNodeRefs.TryAdd(node.NodeRef, 0);
		}
	}

	void RemoveDependencies(ExpressionNode node)
	{
		foreach (var inputNodeRefs in node.InputNodeRefs.Keys)
		{
			if (dictionary.TryGetValue(inputNodeRefs, out var inputNode))
			{
				Logger?.LogTrace("RemoveDependencies {InputNodeRef} and {NodeRef}", inputNode.NodeRef, node.NodeRef);
				inputNode.OutputNodeRefs.TryRemove(node.NodeRef, out _);
			}
		}

		node.InputNodeRefs.Clear();
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			pendingCalculations.Dispose();
		}
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	public void Clear()
	{
		if (isRunning)
		{
			StopCalculationLoop().GetAwaiter().GetResult();
		}
		queuedNodes.Clear();
		while (pendingCalculations.TryTake(out _))
		{
		}

		foreach (var node in dictionary.Values)
		{
			node.Clear();
		}
		dictionary.Clear();
	}
}
