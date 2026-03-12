// JintDemo.xaml.cs

using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.Input;
using Jint;
using Jint.Native;

namespace MauiDevLab;

public partial class JintDemo : ContentPage
{
	[BindableProperty]
	public partial string ScriptText { get; set; }
		= $$"""
		function fetch(url, options = null) {
			return new Promise((resolve, reject) => {
				var xhr = new XMLHttpRequest();
				xhr.responseType = "json";
				xhr.open(options?.method ?? "GET", url, true);
				xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
				if (options.headers && typeof options.headers === "object") {
					for (const [key, value] of Object.entries(options.headers)) {
						xhr.setRequestHeader(key, value);
					}
				}
				xhr.onreadystatechange = function() {
					if (xhr.readyState === XMLHttpRequest.DONE) {
						resolve(xhr);
					}
				};
				xhr.onerror = function(err) {
					reject(err);
				};
				xhr.send(options?.body ?? null);
			});
		}

		async function main() {
			let result = "";
			result += `Add 3 and 4: ${ Add(3,4) }\n`;
			var xhr = await fetch("https://www.arcgis.com/sharing/rest/info", {
				method: "POST",
				body: "f=pjson"
			});
			var json = xhr.responseText;
			result += `Fetched JSON: \n${json}\n`;
			var obj = xhr.response;
			result += `Owning System Url: ${obj.owningSystemUrl}\n`;
			return result;
		}
		""";

	[BindableProperty]
	public partial string ResultText { get; set; } = string.Empty;

	public string InitialScriptText
		= $$"""
		const {
			Add : Add,
			FetchPromiseBridge : FetchAsync,
			TraceWriteLine : TraceWriteLine,
			NewXHR : NewXHR,
		} = __functions;

		const console = {
			log: (...args) => TraceWriteLine(...args),
			error: (...args) => TraceWriteLine("ERROR:", ...args)
		};

		class XMLHttpRequest {
			constructor() {
				this._xhr = NewXHR();
				this._method = "GET";
				this._url = "";
				this._isAsync = true;
				this._error = null;
			}
			open(method, url, isAsync = true) {
				this._method = method;
				this._url = url;
				this._isAsync = isAsync;
				this._xhr.Open(method, url, isAsync);
			}
			setRequestHeader(header, value) {
				this._xhr.SetRequestHeader(header, value);
			}
			send(body = null) {
				if (this._isAsync) {
					this._xhr.SendPromiseBridge(body);
				} else {
					this._xhr.SendSync(body);
				}
			}
			get onreadystatechange() { return this._xhr.OnReadyStateChange; }
			set onreadystatechange(value) { this._xhr.OnReadyStateChange = value; }
			get onerror() { return this._xhr.OnError; }
			set onerror(value) { this._xhr.OnError = value; }
			get readyState() { return this._xhr.ReadyState; }
			get response()
			{
				if (this._xhr.ResponseType == "json")
				{
					return JSON.parse(this._xhr.ResponseText);
				}
				return this._xhr.ResponseText;
			}
			get responseText() { return this._xhr.ResponseText; }
			get responseType() { return this._xhr.ResponseType; }
			set responseType(value) { this._xhr.ResponseType = value; }
			get status() { return this._xhr.StatusCode; }
		}

		XMLHttpRequest.UNSENT = 0;
		XMLHttpRequest.OPENED = 1;
		XMLHttpRequest.HEADERS_RECEIVED = 2;
		XMLHttpRequest.LOADING = 3;
		XMLHttpRequest.DONE = 4;
		""";

	public string ExecuteScriptText
		= $$"""
		(async () => {
			try
			{
				let result = await main();
				__setResult(result);
			}
			catch (err)
			{
				__setError(err?.message ?? String(err));
			}
		})();
		""";

	public JintDemo()
	{
		BindingContext = this;
		InitializeComponent();
	}

	[RelayCommand]
	public async Task Run()
	{
		ResultText = string.Empty;
		using CancellationTokenSource cts = new(TimeSpan.FromSeconds(10));
		JsValue? result = null;
		try
		{
			//cts.Token.Register(() => tcs.TrySetCanceled(cts.Token));
			Jint.Engine engine = new(options => options.CancellationToken(cts.Token));
			JintFunctions functions = new(engine, this, cts.Token);
			engine.SetValue("__functions", functions);
			engine.Execute(InitialScriptText);
			engine.Execute(ScriptText);
			result = engine.Evaluate("(async () => await main())();");
		}
		catch (Exception ex)
		{
			ResultText = "Error: " + ex.Message;
			return;
		}
		result = await Task.Run(() =>
		{
			try
			{
				return result.UnwrapIfPromise();
			}
			catch (Exception ex)
			{
				return "Error: " + ex.Message;
			}
		});
		ResultText = result?.ToString() ?? string.Empty;
	}

	[RelayCommand]
	public async Task RunNew()
	{
		ResultText = string.Empty;
		using CancellationTokenSource cts = new(TimeSpan.FromSeconds(10));
		//cts.Token.Register(() => tcs.TrySetCanceled(cts.Token));
		Jint.Engine engine = new(options => options.CancellationToken(cts.Token));
		JintFunctions functions = new(engine, this, cts.Token);
		engine.SetValue("__functions", functions);
		engine.Execute(InitialScriptText);
		engine.Execute(ScriptText);
		try
		{
			var result = await engine.InvokeAsync<object?>("main");
			ResultText = result?.ToString() ?? "null";
			System.Diagnostics.Trace.WriteLine($"Result from main(): {result}");
		}
		catch (Exception ex)
		{
			ResultText = "Error: " + ex.Message;
		}
	}
}
