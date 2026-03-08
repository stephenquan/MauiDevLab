// JintEngineExtensions.cs

using Jint;
using Jint.Native;

namespace MauiDevLab;

public static class JintEngineExtensions
{
	// --- Core shared implementations ---
	static JsValue ToPromiseInternal(
		Engine engine,
		Func<Task> startTask,
		Func<Task, JsValue> onSuccess,
		SynchronizationContext engineContext)
	{
		ArgumentNullException.ThrowIfNull(engine);
		ArgumentNullException.ThrowIfNull(startTask);
		ArgumentNullException.ThrowIfNull(onSuccess);
		ArgumentNullException.ThrowIfNull(engineContext);

		var (promise, resolve, reject) = engine.Advanced.RegisterPromise();

		Task task;

		try
		{
			task = startTask();
		}
		catch (Exception ex)
		{
			try
			{
				engineContext.Post(_ =>
				{
					var jsError = engine.Intrinsics.Error.Construct(ex.GetBaseException().Message ?? "Unknown error");
					reject(jsError);
				}, null);
			}
			finally
			{
				engine.Advanced.ProcessTasks();

			}
			return promise;
		}

		task.ContinueWith(t => engineContext.Post(_ =>
		{
			try
			{
				if (t.IsFaulted)
				{
					var jsError = engine.Intrinsics.Error.Construct(t.Exception?.GetBaseException().Message ?? "Unknown error");
					reject(jsError);
				}
				else if (t.IsCanceled)
				{
					var jsError = engine.Intrinsics.Error.Construct("Operation canceled");
					reject(jsError);
				}
				else
				{
					resolve(onSuccess(t));
				}
			}
			finally
			{
				engine.Advanced.ProcessTasks();
			}
		}, null), TaskScheduler.Default);

		return promise;
	}

	static JsValue ToFuncPromiseInternal<T>(
		Engine engine,
		Func<Task<T>> invokeAsync,
		SynchronizationContext engineContext)
		=> ToPromiseInternal(engine, () => invokeAsync(), t => JsValue.FromObject(engine, ((Task<T>)t).Result), engineContext);

	static JsValue ToActionPromiseInternal(
		Engine engine,
		Func<Task> invokeAsync,
		SynchronizationContext engineContext)
		=> ToPromiseInternal(engine, () => invokeAsync(), _ => JsValue.Null, engineContext);

	// --- Async function Promise converters ---
	public static JsValue ToPromise<T1, TReturn>(this Engine engine, Func<T1, Task<TReturn>> asyncFunc, T1 p1, SynchronizationContext engineContext)
		=> ToFuncPromiseInternal(engine, () => asyncFunc(p1), engineContext);
	public static JsValue ToPromise<T1, T2, TReturn>(this Engine engine, Func<T1, T2, Task<TReturn>> asyncFunc, T1 p1, T2 p2, SynchronizationContext engineContext)
		=> ToFuncPromiseInternal(engine, () => asyncFunc(p1, p2), engineContext);
	public static JsValue ToPromise<T1, T2, T3, TReturn>(this Engine engine, Func<T1, T2, T3, Task<TReturn>> asyncFunc, T1 p1, T2 p2, T3 p3, SynchronizationContext engineContext)
		=> ToFuncPromiseInternal(engine, () => asyncFunc(p1, p2, p3), engineContext);
	public static JsValue ToPromise<T1, T2, T3, T4, TReturn>(this Engine engine, Func<T1, T2, T3, T4, Task<TReturn>> asyncFunc, T1 p1, T2 p2, T3 p3, T4 p4, SynchronizationContext engineContext)
		=> ToFuncPromiseInternal(engine, () => asyncFunc(p1, p2, p3, p4), engineContext);
	public static JsValue ToPromise<T1, T2, T3, T4, T5, TReturn>(this Engine engine, Func<T1, T2, T3, T4, T5, Task<TReturn>> asyncFunc, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, SynchronizationContext engineContext)
		=> ToFuncPromiseInternal(engine, () => asyncFunc(p1, p2, p3, p4, p5), engineContext);
	public static JsValue ToPromise<T1, T2, T3, T4, T5, T6, TReturn>(this Engine engine, Func<T1, T2, T3, T4, T5, T6, Task<TReturn>> asyncFunc, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, SynchronizationContext engineContext)
		=> ToFuncPromiseInternal(engine, () => asyncFunc(p1, p2, p3, p4, p5, p6), engineContext);

	// --- Async action Promise converters ---
	public static JsValue ToPromise<T1>(this Engine engine, Func<T1, Task> asyncFunc, T1 p1, SynchronizationContext engineContext)
		=> ToActionPromiseInternal(engine, () => asyncFunc(p1), engineContext);
	public static JsValue ToPromise<T1, T2>(this Engine engine, Func<T1, T2, Task> asyncFunc, T1 p1, T2 p2, SynchronizationContext engineContext)
		=> ToActionPromiseInternal(engine, () => asyncFunc(p1, p2), engineContext);
	public static JsValue ToPromise<T1, T2, T3>(this Engine engine, Func<T1, T2, T3, Task> asyncFunc, T1 p1, T2 p2, T3 p3, SynchronizationContext engineContext)
		=> ToActionPromiseInternal(engine, () => asyncFunc(p1, p2, p3), engineContext);
	public static JsValue ToPromise<T1, T2, T3, T4>(this Engine engine, Func<T1, T2, T3, T4, Task> asyncFunc, T1 p1, T2 p2, T3 p3, T4 p4, SynchronizationContext engineContext)
		=> ToActionPromiseInternal(engine, () => asyncFunc(p1, p2, p3, p4), engineContext);
	public static JsValue ToPromise<T1, T2, T3, T4, T5>(this Engine engine, Func<T1, T2, T3, T4, T5, Task> asyncFunc, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, SynchronizationContext engineContext)
		=> ToActionPromiseInternal(engine, () => asyncFunc(p1, p2, p3, p4, p5), engineContext);
	public static JsValue ToPromise<T1, T2, T3, T4, T5, T6>(this Engine engine, Func<T1, T2, T3, T4, T5, T6, Task> asyncFunc, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, SynchronizationContext engineContext)
		=> ToActionPromiseInternal(engine, () => asyncFunc(p1, p2, p3, p4, p5, p6), engineContext);
}
