// CommonFunctions.cs

namespace MauiDevLab;

public class CommonFunctions
{
	protected readonly Page page;
	protected readonly CancellationToken ct;

	public CommonFunctions(Page page, CancellationToken ct)
	{
		System.ArgumentNullException.ThrowIfNull(page, nameof(page));
		this.page = page;
		this.ct = ct;
	}

	public double Add(double x, double y)
	{
		return x + y;
	}

	public static HttpClient HttpClientShared { get; } = new();

	public async Task<string> FetchAsync(string url)
	{
		using var response = await HttpClientShared.GetAsync(url, ct);
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadAsStringAsync(ct);
	}

	public void TraceWriteLine(params object?[] args)
	{
		System.Diagnostics.Trace.WriteLine(string.Join(" ", args));
	}
}
