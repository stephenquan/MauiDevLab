// Fluent.cs

namespace MauiDevLab;

public static class Fluent
{
	public static T Invoke<T>(this T obj, Action<T> action)
	{
		action(obj);
		return obj;
	}
}
