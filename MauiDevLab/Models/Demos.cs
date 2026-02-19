// Demos.cs

namespace MauiDevLab;

public static class Demos
{
	public static Dictionary<string, string> DemoDictionary { get; } = [];
	public static List<string> DemoList => DemoDictionary.Keys.ToList();

	public static MauiAppBuilder RegisterDemo(this MauiAppBuilder builder, string name, string route, Type pageType)
		=> builder.Invoke((b) => { DemoDictionary.Add(name, route); Routing.RegisterRoute(route, pageType); });

	public static string GetDemoRoute(this string name)
		=> DemoDictionary.TryGetValue(name, out var route)
			? route : throw new ArgumentException($"No demo found with name '{name}'");
}
