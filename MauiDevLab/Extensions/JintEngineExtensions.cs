// JintEngineExtensions.cs

using Jint;
using Jint.Native;

namespace MauiDevLab;

public static class JintEngineExtensions
{
	public static void RejectWithMessage(Engine engine, Action<JsValue> reject, string message)
		=> reject(engine.Intrinsics.Error.Construct(message));

	public static void RejectWithException(Engine engine, Action<JsValue> reject, Exception? ex)
		=> RejectWithMessage(engine, reject, ex?.GetBaseException().Message ?? "Unknown error");

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
				try { action(); }
				finally { engine.Advanced.ProcessTasks(); }
			});
		}

		Task task;

		try
		{
			task = startTask();
		}
		catch (Exception ex)
		{
			FinalizeWithTasks(() => RejectWithException(engine, reject, ex));
			return promise;
		}

		_ = task.ContinueWith(t => FinalizeWithTasks(() =>
		{
			if (t.IsFaulted)
			{
				RejectWithException(engine, reject, t.Exception);
			}
			else if (t.IsCanceled)
			{
				RejectWithMessage(engine, reject, "Operation canceled");
			}
			else
			{
				JsValue result;
				try
				{
					result = onSuccess(t);
				}
				catch (Exception ex)
				{
					RejectWithException(engine, reject, ex);
					return;
				}
				resolve(result);
			}
		}), TaskScheduler.Default);

		return promise;
	}

	static JsValue ToFuncPromiseInternal<T>(Engine engine, Func<Task<T>> invokeAsync, Action<Action> finalizePromise)
		=> ToPromiseInternal(engine, () => invokeAsync(), t => JsValue.FromObject(engine, ((Task<T>)t).Result), finalizePromise);

	static JsValue ToActionPromiseInternal(Engine engine, Func<Task> invokeAsync, Action<Action> finalizePromise)
		=> ToPromiseInternal(engine, () => invokeAsync(), _ => JsValue.Null, finalizePromise);

	// --- Async function Promise converters ---
	public static JsValue ToPromise<T1, TReturn>(this Engine engine, Func<T1, Task<TReturn>> asyncFunc, T1 p1, Action<Action> finalizePromise)
		=> ToFuncPromiseInternal(engine, () => asyncFunc(p1), finalizePromise);
	public static JsValue ToPromise<T1, T2, TReturn>(this Engine engine, Func<T1, T2, Task<TReturn>> asyncFunc, T1 p1, T2 p2, Action<Action> finalizePromise)
		=> ToFuncPromiseInternal(engine, () => asyncFunc(p1, p2), finalizePromise);
	public static JsValue ToPromise<T1, T2, T3, TReturn>(this Engine engine, Func<T1, T2, T3, Task<TReturn>> asyncFunc, T1 p1, T2 p2, T3 p3, Action<Action> finalizePromise)
		=> ToFuncPromiseInternal(engine, () => asyncFunc(p1, p2, p3), finalizePromise);
	public static JsValue ToPromise<T1, T2, T3, T4, TReturn>(this Engine engine, Func<T1, T2, T3, T4, Task<TReturn>> asyncFunc, T1 p1, T2 p2, T3 p3, T4 p4, Action<Action> finalizePromise)
		=> ToFuncPromiseInternal(engine, () => asyncFunc(p1, p2, p3, p4), finalizePromise);
	public static JsValue ToPromise<T1, T2, T3, T4, T5, TReturn>(this Engine engine, Func<T1, T2, T3, T4, T5, Task<TReturn>> asyncFunc, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, Action<Action> finalizePromise)
		=> ToFuncPromiseInternal(engine, () => asyncFunc(p1, p2, p3, p4, p5), finalizePromise);
	public static JsValue ToPromise<T1, T2, T3, T4, T5, T6, TReturn>(this Engine engine, Func<T1, T2, T3, T4, T5, T6, Task<TReturn>> asyncFunc, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, Action<Action> finalizePromise)
		=> ToFuncPromiseInternal(engine, () => asyncFunc(p1, p2, p3, p4, p5, p6), finalizePromise);

	// --- Async action Promise converters ---
	public static JsValue ToPromise<T1>(this Engine engine, Func<T1, Task> asyncFunc, T1 p1, Action<Action> finalizePromise)
		=> ToActionPromiseInternal(engine, () => asyncFunc(p1), finalizePromise);
	public static JsValue ToPromise<T1, T2>(this Engine engine, Func<T1, T2, Task> asyncFunc, T1 p1, T2 p2, Action<Action> finalizePromise)
		=> ToActionPromiseInternal(engine, () => asyncFunc(p1, p2), finalizePromise);
	public static JsValue ToPromise<T1, T2, T3>(this Engine engine, Func<T1, T2, T3, Task> asyncFunc, T1 p1, T2 p2, T3 p3, Action<Action> finalizePromise)
		=> ToActionPromiseInternal(engine, () => asyncFunc(p1, p2, p3), finalizePromise);
	public static JsValue ToPromise<T1, T2, T3, T4>(this Engine engine, Func<T1, T2, T3, T4, Task> asyncFunc, T1 p1, T2 p2, T3 p3, T4 p4, Action<Action> finalizePromise)
		=> ToActionPromiseInternal(engine, () => asyncFunc(p1, p2, p3, p4), finalizePromise);
	public static JsValue ToPromise<T1, T2, T3, T4, T5>(this Engine engine, Func<T1, T2, T3, T4, T5, Task> asyncFunc, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, Action<Action> finalizePromise)
		=> ToActionPromiseInternal(engine, () => asyncFunc(p1, p2, p3, p4, p5), finalizePromise);
	public static JsValue ToPromise<T1, T2, T3, T4, T5, T6>(this Engine engine, Func<T1, T2, T3, T4, T5, T6, Task> asyncFunc, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, Action<Action> finalizePromise)
		=> ToActionPromiseInternal(engine, () => asyncFunc(p1, p2, p3, p4, p5, p6), finalizePromise);
}
