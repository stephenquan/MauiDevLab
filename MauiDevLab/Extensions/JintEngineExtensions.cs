// JintEngineExtensions.cs

using Jint;
using Jint.Native;

namespace MauiDevLab;

public static class JintEngineExtensions
{
	// --- Core shared implementations ---
	static JsValue ToFuncPromiseInternal<T>(Engine engine, Func<Task<T>> invokeAsync)
	{
		var scheduler = TaskScheduler.FromCurrentSynchronizationContext();

		var (promise, resolve, reject) = engine.Advanced.RegisterPromise();

		Task<T> task;

		try
		{
			task = invokeAsync();
		}
		catch (Exception ex)
		{
			var jsError = engine.Intrinsics.Error.Construct(ex.GetBaseException().Message ?? "Unknown error");
			reject(jsError);
			return promise;
		}

		task.ContinueWith(t =>
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
				var result = t.Result;
				resolve(JsValue.FromObject(engine, result));
			}
		}, scheduler);

		return promise;
	}

	static JsValue ToActionPromiseInternal(Engine engine, Func<Task> invokeAsync)
	{
		var scheduler = TaskScheduler.FromCurrentSynchronizationContext();

		var (promise, resolve, reject) = engine.Advanced.RegisterPromise();

		Task task;

		try
		{
			task = invokeAsync();
		}
		catch (Exception ex)
		{
			var jsError = engine.Intrinsics.Error.Construct(ex.GetBaseException().Message ?? "Unknown error");
			reject(jsError);
			return promise;
		}

		task.ContinueWith(t =>
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
				resolve(JsValue.Null);
			}
		}, scheduler);

		return promise;
	}

	// --- Async function Promise converters ---
	public static JsValue ToPromise<T1, TReturn>(this Engine engine, Func<T1, Task<TReturn>> asyncFunc, T1 p1)
		=> ToFuncPromiseInternal(engine, () => asyncFunc(p1));
	public static JsValue ToPromise<T1, T2, TReturn>(this Engine engine, Func<T1, T2, Task<TReturn>> asyncFunc, T1 p1, T2 p2)
		=> ToFuncPromiseInternal(engine, () => asyncFunc(p1, p2));
	public static JsValue ToPromise<T1, T2, T3, TReturn>(this Engine engine, Func<T1, T2, T3, Task<TReturn>> asyncFunc, T1 p1, T2 p2, T3 p3)
		=> ToFuncPromiseInternal(engine, () => asyncFunc(p1, p2, p3));
	public static JsValue ToPromise<T1, T2, T3, T4, TReturn>(this Engine engine, Func<T1, T2, T3, T4, Task<TReturn>> asyncFunc, T1 p1, T2 p2, T3 p3, T4 p4)
		=> ToFuncPromiseInternal(engine, () => asyncFunc(p1, p2, p3, p4));
	public static JsValue ToPromise<T1, T2, T3, T4, T5, TReturn>(this Engine engine, Func<T1, T2, T3, T4, T5, Task<TReturn>> asyncFunc, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
		=> ToFuncPromiseInternal(engine, () => asyncFunc(p1, p2, p3, p4, p5));
	public static JsValue ToPromise<T1, T2, T3, T4, T5, T6, TReturn>(this Engine engine, Func<T1, T2, T3, T4, T5, T6, Task<TReturn>> asyncFunc, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6)
		=> ToFuncPromiseInternal(engine, () => asyncFunc(p1, p2, p3, p4, p5, p6));

	// --- Async action Promise converters ---
	public static JsValue ToPromise<T1>(this Engine engine, Func<T1, Task> asyncFunc, T1 p1)
		=> ToActionPromiseInternal(engine, () => asyncFunc(p1));
	public static JsValue ToPromise<T1, T2>(this Engine engine, Func<T1, T2, Task> asyncFunc, T1 p1, T2 p2)
		=> ToActionPromiseInternal(engine, () => asyncFunc(p1, p2));
	public static JsValue ToPromise<T1, T2, T3>(this Engine engine, Func<T1, T2, T3, Task> asyncFunc, T1 p1, T2 p2, T3 p3)
		=> ToActionPromiseInternal(engine, () => asyncFunc(p1, p2, p3));
	public static JsValue ToPromise<T1, T2, T3, T4>(this Engine engine, Func<T1, T2, T3, T4, Task> asyncFunc, T1 p1, T2 p2, T3 p3, T4 p4)
		=> ToActionPromiseInternal(engine, () => asyncFunc(p1, p2, p3, p4));
	public static JsValue ToPromise<T1, T2, T3, T4, T5>(this Engine engine, Func<T1, T2, T3, T4, T5, Task> asyncFunc, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
		=> ToActionPromiseInternal(engine, () => asyncFunc(p1, p2, p3, p4, p5));
	public static JsValue ToPromise<T1, T2, T3, T4, T5, T6>(this Engine engine, Func<T1, T2, T3, T4, T5, T6, Task> asyncFunc, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6)
		=> ToActionPromiseInternal(engine, () => asyncFunc(p1, p2, p3, p4, p5, p6));
}
