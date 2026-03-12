// Safety.cs

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MauiDevLab;

public static class Safety
{
	static TReturn InvokeFuncInternal<TReturn>(Func<TReturn> invoke, string memberName, string filePath, int lineNumber)
	{
		try
		{
			return invoke();
		}
		catch (Exception ex)
		{
			Trace.WriteLine($"[{Path.GetFileNameWithoutExtension(filePath)}.{memberName}:{lineNumber}] {ex}");
#if DEBUG
			throw;
#else
			return default!;
#endif
		}
	}

	static void InvokeActionInternal(Action invoke, string memberName, string filePath, int lineNumber)
	{
		try
		{
			invoke();
		}
		catch (Exception ex)
		{
			Trace.WriteLine($"[{Path.GetFileNameWithoutExtension(filePath)}.{memberName}:{lineNumber}] {ex}");
#if DEBUG
			throw;
#endif
		}
	}

	public static TReturn InvokeFunc<TReturn>(Func<TReturn> func,
		[CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
		=> InvokeFuncInternal(func, memberName, filePath, lineNumber);
	public static TReturn InvokeFunc<T1, TReturn>(Func<T1, TReturn> func, T1 p1,
		[CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
		=> InvokeFuncInternal(() => func(p1), memberName, filePath, lineNumber);
	public static TReturn InvokeFunc<T1, T2, TReturn>(Func<T1, T2, TReturn> func, T1 p1, T2 p2,
		[CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
		=> InvokeFuncInternal(() => func(p1, p2), memberName, filePath, lineNumber);
	public static TReturn InvokeFunc<T1, T2, T3, TReturn>(Func<T1, T2, T3, TReturn> func, T1 p1, T2 p2, T3 p3,
		[CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
		=> InvokeFuncInternal(() => func(p1, p2, p3), memberName, filePath, lineNumber);
	public static TReturn InvokeFunc<T1, T2, T3, T4, TReturn>(Func<T1, T2, T3, T4, TReturn> func, T1 p1, T2 p2, T3 p3, T4 p4,
		[CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
		=> InvokeFuncInternal(() => func(p1, p2, p3, p4), memberName, filePath, lineNumber);
	public static TReturn InvokeFunc<T1, T2, T3, T4, T5, TReturn>(Func<T1, T2, T3, T4, T5, TReturn> func, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5,
		[CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
		=> InvokeFuncInternal(() => func(p1, p2, p3, p4, p5), memberName, filePath, lineNumber);
	public static TReturn InvokeFunc<T1, T2, T3, T4, T5, T6, TReturn>(Func<T1, T2, T3, T4, T5, T6, TReturn> func, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6,
		[CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
		=> InvokeFuncInternal(() => func(p1, p2, p3, p4, p5, p6), memberName, filePath, lineNumber);

	public static void InvokeAction(Action action,
		[CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
		=> InvokeActionInternal(action, memberName, filePath, lineNumber);
	public static void InvokeAction<T1>(Action<T1> action, T1 p1,
		[CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
		=> InvokeActionInternal(() => action(p1), memberName, filePath, lineNumber);
	public static void InvokeAction<T1, T2>(Action<T1, T2> action, T1 p1, T2 p2,
		[CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
		=> InvokeActionInternal(() => action(p1, p2), memberName, filePath, lineNumber);
	public static void InvokeAction<T1, T2, T3>(Action<T1, T2, T3> action, T1 p1, T2 p2, T3 p3,
		[CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
		=> InvokeActionInternal(() => action(p1, p2, p3), memberName, filePath, lineNumber);
	public static void InvokeAction<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 p1, T2 p2, T3 p3, T4 p4,
		[CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
		=> InvokeActionInternal(() => action(p1, p2, p3, p4), memberName, filePath, lineNumber);
	public static void InvokeAction<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5,
		[CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
		=> InvokeActionInternal(() => action(p1, p2, p3, p4, p5), memberName, filePath, lineNumber);
	public static void InvokeAction<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6,
		[CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
		=> InvokeActionInternal(() => action(p1, p2, p3, p4, p5, p6), memberName, filePath, lineNumber);
}
