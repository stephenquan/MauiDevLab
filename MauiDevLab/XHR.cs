// XHR.cs

using System.Net;
using Jint;
using Jint.Native;

namespace MauiDevLab;

public class XHR
{
	Engine engine;
	Page page;
	CancellationToken ct;
	string method = "GET";
	string url = string.Empty;
	bool isAsync = true;

	public string ResponseText { get; set; } = string.Empty;
	public HttpStatusCode StatusCode { get; set; } = 0;

	public XHR(Engine engine, Page page, CancellationToken ct)
	{
		this.engine = engine;
		this.page = page;
		this.ct = ct;
	}

	public void Open(string method, string url, bool isAsync)
	{
		this.method = method;
		this.url = url;
		this.isAsync = isAsync;
	}

	public async Task SendAsync(string? body)
	{
		StatusCode = 0;
		ResponseText = string.Empty;
		using var response = this.method switch
		{
			"POST" => await HttpClientHelper.HttpClientShared.PostAsync(this.url, new StringContent(body ?? string.Empty), ct).ConfigureAwait(false),
			_ => await HttpClientHelper.HttpClientShared.GetAsync(this.url, ct).ConfigureAwait(false),
		};
		ResponseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
		StatusCode = response.StatusCode;
		response.EnsureSuccessStatusCode();
	}

	public void Send(string? body)
	{
		SendAsync(body).GetAwaiter().GetResult();
	}

	public JsValue SendPromiseBridge(string? body)
		=> engine.ToPromise(SendAsync, body, FinalizePromiseWithDispatcher);

	public void FinalizePromiseWithDispatcher(Action action)
	{
		if (!page.Dispatcher.Dispatch(action))
		{
			action();
		}
	}
}
