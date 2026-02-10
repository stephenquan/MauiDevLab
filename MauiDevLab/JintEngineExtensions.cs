// JintEngineExtensions.cs

using Jint;
using Jint.Native;

namespace MauiDevLab;

/// <summary>
/// 
/// </summary>
public static class JintEngineExtensions
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="TReturn"></typeparam>
	/// <param name="jintEngine"></param>
	/// <param name="asyncFunc"></param>
	/// <param name="p1"></param>
	/// <returns></returns>
	public static JsValue ToJsPromise<T1, TReturn>(this Engine jintEngine, Func<T1, Task<TReturn>> asyncFunc, T1 p1)
	{
		var (promise, resolve, reject) = jintEngine.Advanced.RegisterPromise();
		_ = Task.Run(async () =>
		{
			try
			{
				var result = await asyncFunc(p1);
				resolve(JsValue.FromObject(jintEngine, result));
			}
			catch (Exception ex)
			{
				reject(JsValue.FromObject(jintEngine, ex.Message));
			}
		});
		return promise;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="TReturn"></typeparam>
	/// <param name="jintEngine"></param>
	/// <param name="asyncFunc"></param>
	/// <param name="p1"></param>
	/// <param name="p2"></param>
	/// <returns></returns>
	public static JsValue ToJsPromise<T1, T2, TReturn>(this Engine jintEngine, Func<T1, T2, Task<TReturn>> asyncFunc, T1 p1, T2 p2)
	{
		var (promise, resolve, reject) = jintEngine.Advanced.RegisterPromise();
		_ = Task.Run(async () =>
		{
			try
			{
				var result = await asyncFunc(p1, p2);
				resolve(JsValue.FromObject(jintEngine, result));
			}
			catch (Exception ex)
			{
				reject(JsValue.FromObject(jintEngine, ex.Message));
			}
		});
		return promise;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="TReturn"></typeparam>
	/// <param name="jintEngine"></param>
	/// <param name="asyncFunc"></param>
	/// <param name="p1"></param>
	/// <param name="p2"></param>
	/// <param name="p3"></param>
	/// <returns></returns>
	public static JsValue ToJsPromise<T1, T2, T3, TReturn>(this Engine jintEngine, Func<T1, T2, T3, Task<TReturn>> asyncFunc, T1 p1, T2 p2, T3 p3)
	{
		var (promise, resolve, reject) = jintEngine.Advanced.RegisterPromise();
		_ = Task.Run(async () =>
		{
			try
			{
				var result = await asyncFunc(p1, p2, p3);
				resolve(JsValue.FromObject(jintEngine, result));
			}
			catch (Exception ex)
			{
				reject(JsValue.FromObject(jintEngine, ex.Message));
			}
		});
		return promise;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="TReturn"></typeparam>
	/// <param name="jintEngine"></param>
	/// <param name="asyncFunc"></param>
	/// <returns></returns>
	public static Func<T1, Task<JsValue>> ToJsPromiseFunc<T1, TReturn>(this Engine jintEngine, Func<T1, Task<TReturn>> asyncFunc)
		=> async (p1) => ToJsPromise(jintEngine, asyncFunc, p1);

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="TReturn"></typeparam>
	/// <param name="jintEngine"></param>
	/// <param name="asyncFunc"></param>
	/// <returns></returns>
	public static Func<T1, T2, Task<JsValue>> ToJsPromiseFunc<T1, T2, TReturn>(this Engine jintEngine, Func<T1, T2, Task<TReturn>> asyncFunc)
		=> async (p1, p2) => ToJsPromise(jintEngine, asyncFunc, p1, p2);

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="TReturn"></typeparam>
	/// <param name="jintEngine"></param>
	/// <param name="asyncFunc"></param>
	/// <returns></returns>
	public static Func<T1, T2, T3, Task<JsValue>> ToJsPromiseFunc<T1, T2, T3, TReturn>(this Engine jintEngine, Func<T1, T2, T3, Task<TReturn>> asyncFunc)
		=> async (p1, p2, p3) => ToJsPromise(jintEngine, asyncFunc, p1, p2, p3);

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="TReturn"></typeparam>
	/// <param name="jintEngine"></param>
	/// <param name="name"></param>
	/// <param name="asyncFunc"></param>
	/// <returns></returns>
	public static Engine SetFuncValue<T1, TReturn>(this Engine jintEngine, string name, Func<T1, Task<TReturn>> asyncFunc)
		=> jintEngine.SetValue(name, ToJsPromiseFunc(jintEngine, asyncFunc));

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="TReturn"></typeparam>
	/// <param name="jintEngine"></param>
	/// <param name="name"></param>
	/// <param name="asyncFunc"></param>
	/// <returns></returns>
	public static Engine SetFuncValue<T1, T2, TReturn>(this Engine jintEngine, string name, Func<T1, T2, Task<TReturn>> asyncFunc)
		=> jintEngine.SetValue(name, ToJsPromiseFunc(jintEngine, asyncFunc));

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	/// <typeparam name="T3"></typeparam>
	/// <typeparam name="TReturn"></typeparam>
	/// <param name="jintEngine"></param>
	/// <param name="name"></param>
	/// <param name="asyncFunc"></param>
	/// <returns></returns>
	public static Engine SetFuncValue<T1, T2, T3, TReturn>(this Engine jintEngine, string name, Func<T1, T2, T3, Task<TReturn>> asyncFunc)
		=> jintEngine.SetValue(name, ToJsPromiseFunc(jintEngine, asyncFunc));
}
