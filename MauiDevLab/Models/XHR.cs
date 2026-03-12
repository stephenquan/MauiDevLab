// XHR.cs

using System.Text;
using Jint;
using Jint.Native;

namespace MauiDevLab;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
	"Usage",
	"CA1001:Types that own disposable fields should be disposable",
	Justification = "XHR manages request/response lifetime internally after each request.")]
public class XHR
{
	Engine engine;
	Page page;
	CancellationToken ct;

	HttpRequestMessage? request;
	const string defaultMediaType = "text/plain";
	string mediaType = defaultMediaType;
	public ReadyStateEnum ReadyState { get; private set; } = ReadyStateEnum.UNSENT;
	public Action? OnReadyStateChange;
	public Action<JsValue>? OnError;
	public string ResponseType { get; set; } = string.Empty;
	public string ResponseText { get; set; } = string.Empty;

	public int StatusCode { get; set; } = 0;

	public XHR(Engine engine, Page page, CancellationToken ct)
	{
		this.engine = engine;
		this.page = page;
		this.ct = ct;
	}

	public void Reset()
	{
		request?.Dispose();
		request = null;
		StatusCode = 0;
		ResponseText = string.Empty;
		ReadyState = ReadyStateEnum.UNSENT;
		mediaType = defaultMediaType;
		FinalizePromiseWithDispatcher(() => OnReadyStateChange?.Invoke());
	}

	public void Open(string method, string url, bool isasync = true)
	{
		Reset();
		request = new HttpRequestMessage(new HttpMethod(method), url);
		ReadyState = ReadyStateEnum.OPENED;
		FinalizePromiseWithDispatcher(() => OnReadyStateChange?.Invoke());
	}

	public void SetRequestHeader(string header, string value)
	{
		ArgumentNullException.ThrowIfNull(request, "Request not initialized. Call Open() first.");
		request.Headers.TryAddWithoutValidation(header, value);
		if (header.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
		{
			mediaType = value;
		}
	}

	public async Task SendAsync(string? body)
	{
		ArgumentNullException.ThrowIfNull(request, "Request not initialized. Call Open() first.");

		try
		{
			if (body != null && (request.Method == HttpMethod.Post || request.Method == HttpMethod.Put || request.Method == HttpMethod.Patch))
			{
				request.Content = new StringContent(body, Encoding.UTF8, mediaType);
			}

			using var response = await HttpClientHelper.HttpClientShared
				.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct)
				.ConfigureAwait(false);
			StatusCode = (int)response.StatusCode;
			ReadyState = ReadyStateEnum.HEADERS_RECEIVED;
			OnReadyStateChange?.Invoke();
			ReadyState = ReadyStateEnum.LOADING;
			OnReadyStateChange?.Invoke();
			ResponseText = await response.Content
				.ReadAsStringAsync(ct)
				.ConfigureAwait(false);
			request?.Dispose();
			request = null;
			ReadyState = ReadyStateEnum.DONE;
			OnReadyStateChange?.Invoke();
		}
		catch (Exception ex)
		{
			StatusCode = 0;
			request?.Dispose();
			request = null;
			var error = engine.NewErrorFromException(ex);
			FinalizePromiseWithDispatcher(() => OnError?.Invoke(error));
			ReadyState = ReadyStateEnum.DONE;
			FinalizePromiseWithDispatcher(() => OnReadyStateChange?.Invoke());
			throw;
		}
	}

	public JsValue SendPromiseBridge(string? body)
		=> engine.ToActionPromise(SendAsync, body, FinalizePromiseWithDispatcher);

	public void SendSync(string? body = null)
	{
		SendAsync(body).GetAwaiter().GetResult();
	}

	public void FinalizePromiseWithDispatcher(Action action)
	{
		if (!page.Dispatcher.Dispatch(action))
		{
			action();
		}
	}
}

public enum ReadyStateEnum
{
	UNSENT = 0,
	OPENED = 1,
	HEADERS_RECEIVED = 2,
	LOADING = 3,
	DONE = 4
}
