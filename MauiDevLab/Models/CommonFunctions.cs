// CommonFunctions.cs

namespace MauiDevLab;

public class CommonFunctions
{
	protected Page page;

	public CommonFunctions(Page page)
	{
		this.page = page;
	}

	public double Add(double x, double y)
	{
		return x + y;
	}

	public HttpClient HttpClientShared { get; } = new();

	public async Task<string> FetchAsync(string url)
	{
		using var response = await HttpClientShared.GetAsync(url);
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadAsStringAsync();
	}

	public void TraceWriteLine(params object?[] args)
	{
		System.Diagnostics.Trace.WriteLine(string.Join(" ", args));
	}
}
