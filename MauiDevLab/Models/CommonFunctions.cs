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

	public async Task<string> FetchAsync(string url)
	{
		using var response = await HttpClientHelper.HttpClientShared.GetAsync(url, ct).ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
	}

	public void TraceWriteLine(params object?[] args)
	{
		System.Diagnostics.Trace.WriteLine(string.Join(" ", args));
	}
}
