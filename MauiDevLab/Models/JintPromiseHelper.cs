// JintPromiseHelper.cs

using Jint;
using Jint.Native;

namespace MauiDevLab;


public class JintPromiseHelper
{
	public Engine Engine { get; }
	public Action<Action> FinalizePromise { get; }
	public JintPromiseHelper(Engine engine, Action<Action> finalizePromise)
	{
		this.Engine = engine;
		this.FinalizePromise = finalizePromise;
	}
	public JintPromiseHelper(Engine engine, SynchronizationContext? context) : this(
		engine,
		(context is null)
			? a => a()
			: a => context?.Post(_ => a(), null))
	{
	}

	public Func<JsValue> NewFuncPromise<TReturn>(Func<Task<TReturn>> func)
		=> this.Engine.NewFuncPromise(func, this.FinalizePromise);
	public Func<T1, JsValue> NewFuncPromise<T1, TReturn>(Func<T1, Task<TReturn>> func)
		=> this.Engine.NewFuncPromise(func, this.FinalizePromise);
	public Func<T1, T2, JsValue> NewFuncPromise<T1, T2, TReturn>(Func<T1, T2, Task<TReturn>> func)
		=> this.Engine.NewFuncPromise(func, this.FinalizePromise);
	public Func<T1, T2, T3, JsValue> NewFuncPromise<T1, T2, T3, TReturn>(Func<T1, T2, T3, Task<TReturn>> func)
		=> this.Engine.NewFuncPromise(func, this.FinalizePromise);
	public Func<T1, T2, T3, T4, JsValue> NewFuncPromise<T1, T2, T3, T4, TReturn>(Func<T1, T2, T3, T4, Task<TReturn>> func)
		=> this.Engine.NewFuncPromise(func, this.FinalizePromise);
	public Func<T1, T2, T3, T4, T5, JsValue> NewFuncPromise<T1, T2, T3, T4, T5, TReturn>(Func<T1, T2, T3, T4, T5, Task<TReturn>> func)
		=> this.Engine.NewFuncPromise(func, this.FinalizePromise);
	public Func<T1, T2, T3, T4, T5, T6, JsValue> NewFuncPromise<T1, T2, T3, T4, T5, T6, TReturn>(Func<T1, T2, T3, T4, T5, T6, Task<TReturn>> func)
		=> this.Engine.NewFuncPromise(func, this.FinalizePromise);

	public Func<JsValue> NewActionPromise(Func<Task> action)
		=> this.Engine.NewActionPromise(action, FinalizePromise);
	public Func<T1, JsValue> NewActionPromise<T1>(Func<T1, Task> action)
		=> this.Engine.NewActionPromise(action, this.FinalizePromise);
	public Func<T1, T2, JsValue> NewActionPromise<T1, T2>(Func<T1, T2, Task> action)
		=> this.Engine.NewActionPromise(action, this.FinalizePromise);
	public Func<T1, T2, T3, JsValue> NewActionPromise<T1, T2, T3>(Func<T1, T2, T3, Task> action)
		=> this.Engine.NewActionPromise(action, this.FinalizePromise);
	public Func<T1, T2, T3, T4, JsValue> NewActionPromise<T1, T2, T3, T4>(Func<T1, T2, T3, T4, Task> action)
		=> this.Engine.NewActionPromise(action, this.FinalizePromise);
	public Func<T1, T2, T3, T4, T5, JsValue> NewActionPromise<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, Task> action)
		=> this.Engine.NewActionPromise(action, this.FinalizePromise);
	public Func<T1, T2, T3, T4, T5, T6, JsValue> NewActionPromise<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, Task> action)
		=> this.Engine.NewActionPromise(action, this.FinalizePromise);
}
