// XHR.cs

using System.Net;
using Jint;
using Jint.Native;

namespace MauiDevLab;

public class XHR
{
	Engine engine;
	Page page;
	string method = "GET";
	string url = string.Empty;
	bool isAsync = true;

	public static HttpClient HttpClientShared { get; } = new();

	public string ResponseText { get; set; } = string.Empty;
	public HttpStatusCode StatusCode { get; set; } = 0;

	public XHR(Engine engine, Page page)
	{
		this.engine = engine;
		this.page = page;
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
		using var response = await HttpClientShared.GetAsync(this.url).ConfigureAwait(false);
		response.EnsureSuccessStatusCode();
		ResponseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
		StatusCode = response.StatusCode;
	}

	public void Send(string? body)
	{
		SendAsync(body).GetAwaiter().GetResult();
	}

	public JsValue SendPromiseBridge(string? body)
		=> engine.ToPromise(SendAsync, body, FinalizePromiseWithDispatcher);

	public void FinalizePromiseWithDispatcher(Action action)
		=> page.Dispatcher.Dispatch(action);
}
