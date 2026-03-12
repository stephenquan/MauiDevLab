// JintEngineExtensions.cs

using Jint;
using Jint.Native;

namespace MauiDevLab;

public static class JintEngineExtensions
{
	public static Jint.Native.Object.ObjectInstance NewErrorFromMessage(this Engine engine, string message)
		=> engine.Intrinsics.Error.Construct(message);
	public static Jint.Native.Object.ObjectInstance NewErrorFromException(this Engine engine, Exception? ex)
		=> engine.NewErrorFromMessage(ex?.GetBaseException().Message ?? "Unknown error");

	public static void RejectPromiseWithMessage(Engine engine, Action<JsValue> reject, string message)
		=> reject(engine.NewErrorFromMessage(message));

	public static void RejectPromiseWithException(Engine engine, Action<JsValue> reject, Exception? ex)
		=> reject(engine.NewErrorFromException(ex));

	// --- Core shared implementations ---
	static JsValue ToPromiseInternal(Engine engine, Func<Task> startTask, Func<Task, JsValue> onSuccess, Action<Action> finalizePromise)
	{
		ArgumentNullException.ThrowIfNull(engine);
		ArgumentNullException.ThrowIfNull(startTask);
		ArgumentNullException.ThrowIfNull(onSuccess);
		ArgumentNullException.ThrowIfNull(finalizePromise);

		var (promise, resolve, reject) = engine.Advanced.RegisterPromise();

		void FinalizeWithTasks(Action action)
		{
			finalizePromise(() =>
			{
				try
				{
					action();
				}
				catch (Exception ex)
				{
					// last-chance reject to avoid a forever-pending promise
					reject(engine.NewErrorFromException(ex));
				}
				finally
				{
					try
					{
						engine.Advanced.ProcessTasks();
					}
					catch
					{
						// swallow or log
					}
				}

			});
		}

		async Task RunAsync()
		{
			try
			{
				var task = startTask();
				await task.ConfigureAwait(false);
				JsValue result = onSuccess(task);
				FinalizeWithTasks(() => resolve(result));
			}
			catch (Exception ex)
			{
				FinalizeWithTasks(() => RejectPromiseWithException(engine, reject, ex));
			}
		}

		_ = RunAsync();

		return promise;
	}

	static JsValue ToFuncPromiseInternal<T>(Engine engine, Func<Task<T>> func, Action<Action> finalizePromise)
		=> ToPromiseInternal(engine, () => func(), t => JsValue.FromObject(engine, ((Task<T>)t).Result), finalizePromise);

	static JsValue ToActionPromiseInternal(Engine engine, Func<Task> action, Action<Action> finalizePromise)
		=> ToPromiseInternal(engine, () => action(), _ => JsValue.Null, finalizePromise);

	// --- Async function Promise converters ---
	public static JsValue ToFuncPromise<TReturn>(this Engine engine, Func<Task<TReturn>> func, Action<Action> finalizePromise)
		=> ToFuncPromiseInternal(engine, () => func(), finalizePromise);
	public static JsValue ToFuncPromise<T1, TReturn>(this Engine engine, Func<T1, Task<TReturn>> func, T1 p1, Action<Action> finalizePromise)
		=> ToFuncPromiseInternal(engine, () => func(p1), finalizePromise);
	public static JsValue ToFuncPromise<T1, T2, TReturn>(this Engine engine, Func<T1, T2, Task<TReturn>> func, T1 p1, T2 p2, Action<Action> finalizePromise)
		=> ToFuncPromiseInternal(engine, () => func(p1, p2), finalizePromise);
	public static JsValue ToFuncPromise<T1, T2, T3, TReturn>(this Engine engine, Func<T1, T2, T3, Task<TReturn>> func, T1 p1, T2 p2, T3 p3, Action<Action> finalizePromise)
		=> ToFuncPromiseInternal(engine, () => func(p1, p2, p3), finalizePromise);
	public static JsValue ToFuncPromise<T1, T2, T3, T4, TReturn>(this Engine engine, Func<T1, T2, T3, T4, Task<TReturn>> func, T1 p1, T2 p2, T3 p3, T4 p4, Action<Action> finalizePromise)
		=> ToFuncPromiseInternal(engine, () => func(p1, p2, p3, p4), finalizePromise);
	public static JsValue ToFuncPromise<T1, T2, T3, T4, T5, TReturn>(this Engine engine, Func<T1, T2, T3, T4, T5, Task<TReturn>> func, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, Action<Action> finalizePromise)
		=> ToFuncPromiseInternal(engine, () => func(p1, p2, p3, p4, p5), finalizePromise);
	public static JsValue ToFuncPromise<T1, T2, T3, T4, T5, T6, TReturn>(this Engine engine, Func<T1, T2, T3, T4, T5, T6, Task<TReturn>> func, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, Action<Action> finalizePromise)
		=> ToFuncPromiseInternal(engine, () => func(p1, p2, p3, p4, p5, p6), finalizePromise);

	// --- Async action Promise converters ---
	public static JsValue ToActionPromise(this Engine engine, Func<Task> func, Action<Action> finalizePromise)
		=> ToActionPromiseInternal(engine, () => func(), finalizePromise);
	public static JsValue ToActionPromise<T1>(this Engine engine, Func<T1, Task> func, T1 p1, Action<Action> finalizePromise)
		=> ToActionPromiseInternal(engine, () => func(p1), finalizePromise);
	public static JsValue ToActionPromise<T1, T2>(this Engine engine, Func<T1, T2, Task> func, T1 p1, T2 p2, Action<Action> finalizePromise)
		=> ToActionPromiseInternal(engine, () => func(p1, p2), finalizePromise);
	public static JsValue ToActionPromise<T1, T2, T3>(this Engine engine, Func<T1, T2, T3, Task> func, T1 p1, T2 p2, T3 p3, Action<Action> finalizePromise)
		=> ToActionPromiseInternal(engine, () => func(p1, p2, p3), finalizePromise);
	public static JsValue ToActionPromise<T1, T2, T3, T4>(this Engine engine, Func<T1, T2, T3, T4, Task> func, T1 p1, T2 p2, T3 p3, T4 p4, Action<Action> finalizePromise)
		=> ToActionPromiseInternal(engine, () => func(p1, p2, p3, p4), finalizePromise);
	public static JsValue ToActionPromise<T1, T2, T3, T4, T5>(this Engine engine, Func<T1, T2, T3, T4, T5, Task> func, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, Action<Action> finalizePromise)
		=> ToActionPromiseInternal(engine, () => func(p1, p2, p3, p4, p5), finalizePromise);
	public static JsValue ToActionPromise<T1, T2, T3, T4, T5, T6>(this Engine engine, Func<T1, T2, T3, T4, T5, T6, Task> func, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, Action<Action> finalizePromise)
		=> ToActionPromiseInternal(engine, () => func(p1, p2, p3, p4, p5, p6), finalizePromise);

	// --- NewFuncPromise  ---
	public static Func<JsValue> NewFuncPromise<TReturn>(this Engine engine, Func<Task<TReturn>> func, Action<Action> finalizePromise)
		=> () => engine.ToFuncPromise(func, finalizePromise);
	public static Func<T1, JsValue> NewFuncPromise<T1, TReturn>(this Engine engine, Func<T1, Task<TReturn>> func, Action<Action> finalizePromise)
		=> p1 => engine.ToFuncPromise(func, p1, finalizePromise);
	public static Func<T1, T2, JsValue> NewFuncPromise<T1, T2, TReturn>(this Engine engine, Func<T1, T2, Task<TReturn>> func, Action<Action> finalizePromise)
		=> (p1, p2) => engine.ToFuncPromise(func, p1, p2, finalizePromise);
	public static Func<T1, T2, T3, JsValue> NewFuncPromise<T1, T2, T3, TReturn>(this Engine engine, Func<T1, T2, T3, Task<TReturn>> func, Action<Action> finalizePromise)
		=> (p1, p2, p3) => engine.ToFuncPromise(func, p1, p2, p3, finalizePromise);
	public static Func<T1, T2, T3, T4, JsValue> NewFuncPromise<T1, T2, T3, T4, TReturn>(this Engine engine, Func<T1, T2, T3, T4, Task<TReturn>> func, Action<Action> finalizePromise)
		=> (p1, p2, p3, p4) => engine.ToFuncPromise(func, p1, p2, p3, p4, finalizePromise);
	public static Func<T1, T2, T3, T4, T5, JsValue> NewFuncPromise<T1, T2, T3, T4, T5, TReturn>(this Engine engine, Func<T1, T2, T3, T4, T5, Task<TReturn>> func, Action<Action> finalizePromise)
		=> (p1, p2, p3, p4, p5) => engine.ToFuncPromise(func, p1, p2, p3, p4, p5, finalizePromise);
	public static Func<T1, T2, T3, T4, T5, T6, JsValue> NewFuncPromise<T1, T2, T3, T4, T5, T6, TReturn>(this Engine engine, Func<T1, T2, T3, T4, T5, T6, Task<TReturn>> func, Action<Action> finalizePromise)
		=> (p1, p2, p3, p4, p5, p6) => engine.ToFuncPromise(func, p1, p2, p3, p4, p5, p6, finalizePromise);

	// --- NewActionPromise  ---
	public static Func<JsValue> NewActionPromise(this Engine engine, Func<Task> action, Action<Action> finalizePromise)
		=> () => engine.ToActionPromise(action, finalizePromise);
	public static Func<T1, JsValue> NewActionPromise<T1>(this Engine engine, Func<T1, Task> action, Action<Action> finalizePromise)
		=> (p1) => engine.ToActionPromise(action, p1, finalizePromise);
	public static Func<T1, T2, JsValue> NewActionPromise<T1, T2>(this Engine engine, Func<T1, T2, Task> action, Action<Action> finalizePromise)
		=> (p1, p2) => engine.ToActionPromise(action, p1, p2, finalizePromise);
	public static Func<T1, T2, T3, JsValue> NewActionPromise<T1, T2, T3>(this Engine engine, Func<T1, T2, T3, Task> action, Action<Action> finalizePromise)
		=> (p1, p2, p3) => engine.ToActionPromise(action, p1, p2, p3, finalizePromise);
	public static Func<T1, T2, T3, T4, JsValue> NewActionPromise<T1, T2, T3, T4>(this Engine engine, Func<T1, T2, T3, T4, Task> action, Action<Action> finalizePromise)
		=> (p1, p2, p3, p4) => engine.ToActionPromise(action, p1, p2, p3, p4, finalizePromise);
	public static Func<T1, T2, T3, T4, T5, JsValue> NewActionPromise<T1, T2, T3, T4, T5>(this Engine engine, Func<T1, T2, T3, T4, T5, Task> action, Action<Action> finalizePromise)
		=> (p1, p2, p3, p4, p5) => engine.ToActionPromise(action, p1, p2, p3, p4, p5, finalizePromise);
	public static Func<T1, T2, T3, T4, T5, T6, JsValue> NewActionPromise<T1, T2, T3, T4, T5, T6>(this Engine engine, Func<T1, T2, T3, T4, T5, T6, Task> action, Action<Action> finalizePromise)
		=> (p1, p2, p3, p4, p5, p6) => engine.ToActionPromise(action, p1, p2, p3, p4, p5, p6, finalizePromise);

	// -- EvaluateAsync --
	public static async Task<T> EvaluateAsync<T>(this Engine engine, string script)
	{
		ArgumentNullException.ThrowIfNull(engine);
		ArgumentNullException.ThrowIfNull(script);
		TaskCompletionSource<T> tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
		engine.SetValue("__setResult", new Action<T>(result => tcs.TrySetResult(result)));
		engine.SetValue("__setError", new Action<string>(error => tcs.TrySetException(new Exception(error))));
		var result = engine.Execute(
			$$"""
            {{script}}
            (async () => {
                try {
                    var result = await main();
                    __setResult(result);
                } catch (err) {
                    __setError(String(err));
                }
            })();
            """);
		engine.Advanced.ProcessTasks();
		return await tcs.Task.ConfigureAwait(false);
	}

	// -- InvokeAsync --
	public static async Task<T> InvokeAsync<T>(this Engine engine, string functionName, params object[] args)
	{
		ArgumentNullException.ThrowIfNull(engine);
		ArgumentNullException.ThrowIfNull(functionName);
		TaskCompletionSource<T> tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
		engine.SetValue("__setResult", new Action<T>(result => tcs.TrySetResult(result)));
		engine.SetValue("__setError", new Action<string>(error => tcs.TrySetException(new Exception(error))));
		engine.SetValue("__functionName", functionName);
		engine.SetValue("__functionArgs", args);
		engine.Execute(
			$$"""
            (async () => {
                try {
                    const fn = globalThis[__functionName];
                    const result = await fn(... __functionArgs);
                    __setResult(result);
                } catch (err) {
                    __setError(String(err));
                }
            })();
            """
			);
		engine.Advanced.ProcessTasks();
		return await tcs.Task.ConfigureAwait(false);
	}
}

public class JintPromiseHelpers
{
	public Engine Engine { get; }
	public Action<Action> FinalizePromise { get; }
	public JintPromiseHelpers(Engine engine, Action<Action> finalizePromise)
	{
		this.Engine = engine;
		this.FinalizePromise = finalizePromise;
	}
	public JintPromiseHelpers(Engine engine, SynchronizationContext? context) : this(
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
