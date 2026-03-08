// JintEngineExtensions.cs

using Jint;
using Jint.Native;

namespace MauiDevLab;

public static class JintEngineExtensions
{
	// --- Core shared implementations ---
	static JsValue ToFuncPromiseInternal<T>(Engine engine, Func<Task<T>> invokeAsync, Action<Action>? finalizePromise = null)
	{
		var context = SynchronizationContext.Current;

		finalizePromise ??= action =>
		{
			if (context is not null)
			{
				context.Post(_ => action(), null);
			}
			else
			{
				action();
			}
		};

		var (promise, resolve, reject) = engine.Advanced.RegisterPromise();

		Task<T> task;

		try
		{
			task = invokeAsync();
		}
		catch (Exception ex)
		{
			var jsError = engine.Intrinsics.Error.Construct(ex.GetBaseException().Message ?? "Unknown error");
			finalizePromise(() => reject(jsError));
			return promise;
		}

		task.ContinueWith(t =>
		{
			if (t.IsFaulted)
			{
				var jsError = engine.Intrinsics.Error.Construct(t.Exception?.GetBaseException().Message ?? "Unknown error");
				finalizePromise(() => reject(jsError));
			}
			else if (t.IsCanceled)
			{
				var jsError = engine.Intrinsics.Error.Construct("Operation canceled");
				finalizePromise(() => reject(jsError));
			}
			else
			{
				var result = t.Result;
				finalizePromise(() => resolve(JsValue.FromObject(engine, result)));
			}
		});

		return promise;
	}

	static JsValue ToActionPromiseInternal(Engine engine, Func<Task> invokeAsync, Action<Action>? finalizePromise = null)
	{
		var context = SynchronizationContext.Current;

		finalizePromise ??= action =>
		{
			if (context is not null)
			{
				context.Post(_ => action(), null);
			}
			else
			{
				action();
			}
		};

		var (promise, resolve, reject) = engine.Advanced.RegisterPromise();

		Task task;

		try
		{
			task = invokeAsync();
		}
		catch (Exception ex)
		{
			var jsError = engine.Intrinsics.Error.Construct(ex.GetBaseException().Message ?? "Unknown error");
			finalizePromise(() => reject(jsError));
			return promise;
		}

		task.ContinueWith(t =>
		{
			if (t.IsFaulted)
			{
				var jsError = engine.Intrinsics.Error.Construct(t.Exception?.GetBaseException().Message ?? "Unknown error");
				finalizePromise(() => reject(jsError));
			}
			else if (t.IsCanceled)
			{
				var jsError = engine.Intrinsics.Error.Construct("Operation canceled");
				finalizePromise(() => reject(jsError));
			}
			else
			{
				finalizePromise(() => resolve(JsValue.Null));
			}
		});

		return promise;
	}

	// --- Async function Promise converters ---
	public static JsValue ToPromise<T1, TReturn>(this Engine engine, Func<T1, Task<TReturn>> asyncFunc, T1 p1, Action<Action>? finalizePromise = null)
		=> ToFuncPromiseInternal(engine, () => asyncFunc(p1), finalizePromise);
	public static JsValue ToPromise<T1, T2, TReturn>(this Engine engine, Func<T1, T2, Task<TReturn>> asyncFunc, T1 p1, T2 p2, Action<Action>? finalizePromise = null)
		=> ToFuncPromiseInternal(engine, () => asyncFunc(p1, p2), finalizePromise);
	public static JsValue ToPromise<T1, T2, T3, TReturn>(this Engine engine, Func<T1, T2, T3, Task<TReturn>> asyncFunc, T1 p1, T2 p2, T3 p3, Action<Action>? finalizePromise = null)
		=> ToFuncPromiseInternal(engine, () => asyncFunc(p1, p2, p3), finalizePromise);
	public static JsValue ToPromise<T1, T2, T3, T4, TReturn>(this Engine engine, Func<T1, T2, T3, T4, Task<TReturn>> asyncFunc, T1 p1, T2 p2, T3 p3, T4 p4, Action<Action>? finalizePromise = null)
		=> ToFuncPromiseInternal(engine, () => asyncFunc(p1, p2, p3, p4), finalizePromise);
	public static JsValue ToPromise<T1, T2, T3, T4, T5, TReturn>(this Engine engine, Func<T1, T2, T3, T4, T5, Task<TReturn>> asyncFunc, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, Action<Action>? finalizePromise = null)
		=> ToFuncPromiseInternal(engine, () => asyncFunc(p1, p2, p3, p4, p5), finalizePromise);
	public static JsValue ToPromise<T1, T2, T3, T4, T5, T6, TReturn>(this Engine engine, Func<T1, T2, T3, T4, T5, T6, Task<TReturn>> asyncFunc, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, Action<Action>? finalizePromise = null)
		=> ToFuncPromiseInternal(engine, () => asyncFunc(p1, p2, p3, p4, p5, p6), finalizePromise);

	// --- Async action Promise converters ---
	public static JsValue ToPromise<T1>(this Engine engine, Func<T1, Task> asyncFunc, T1 p1, Action<Action>? finalizePromise = null)
		=> ToActionPromiseInternal(engine, () => asyncFunc(p1), finalizePromise);
	public static JsValue ToPromise<T1, T2>(this Engine engine, Func<T1, T2, Task> asyncFunc, T1 p1, T2 p2, Action<Action>? finalizePromise = null)
		=> ToActionPromiseInternal(engine, () => asyncFunc(p1, p2), finalizePromise);
	public static JsValue ToPromise<T1, T2, T3>(this Engine engine, Func<T1, T2, T3, Task> asyncFunc, T1 p1, T2 p2, T3 p3, Action<Action>? finalizePromise = null)
		=> ToActionPromiseInternal(engine, () => asyncFunc(p1, p2, p3), finalizePromise);
	public static JsValue ToPromise<T1, T2, T3, T4>(this Engine engine, Func<T1, T2, T3, T4, Task> asyncFunc, T1 p1, T2 p2, T3 p3, T4 p4, Action<Action>? finalizePromise = null)
		=> ToActionPromiseInternal(engine, () => asyncFunc(p1, p2, p3, p4), finalizePromise);
	public static JsValue ToPromise<T1, T2, T3, T4, T5>(this Engine engine, Func<T1, T2, T3, T4, T5, Task> asyncFunc, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, Action<Action>? finalizePromise = null)
		=> ToActionPromiseInternal(engine, () => asyncFunc(p1, p2, p3, p4, p5), finalizePromise);
	public static JsValue ToPromise<T1, T2, T3, T4, T5, T6>(this Engine engine, Func<T1, T2, T3, T4, T5, T6, Task> asyncFunc, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, Action<Action>? finalizePromise = null)
		=> ToActionPromiseInternal(engine, () => asyncFunc(p1, p2, p3, p4, p5, p6), finalizePromise);
}
