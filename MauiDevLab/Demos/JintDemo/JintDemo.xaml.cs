// JintDemo.xaml.cs

using System.Text;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.Input;
using Jint;

namespace MauiDevLab;

public partial class JintDemo : ContentPage
{
	[BindableProperty]
	public partial string ScriptText { get; set; }
		= $$"""
		async function main() {
			let a = 3;
			let b = 4;
			console.log("Add 3 and 4: ", Add(a,b));
			var h = Add(a,b);
			console.log("new XHR");
			var xhr = new XMLHttpRequest();
			xhr.onreadystatechange = function() {
				console.log("Ready state changed: ", xhr.readyState);
			};
			xhr.open("GET", "https://www.arcgis.com/sharing/rest/info?f=pjson");
			xhr.send();
			var json = xhr.responseText;
			console.log("Fetched JSON: ", json);
			var obj = JSON.parse(json);
			console.log("Owning System Url: ", obj.owningSystemUrl);
			return json;
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
				this.readyState = XMLHttpRequest.UNSENT;
				this.onreadystatechange = null;
			}
			open(method, url, isAsync = true) {
				this._method = method;
				this._url = url;
				this._isAsync = isAsync;
				this._xhr.Open(method, url, isAsync);
				this.readyState = XMLHttpRequest.OPENED;
				if (this.onreadystatechange) this.onreadystatechange();
			}
			send(body = null) {
				this._xhr.Send(body);
				this.readyState = XMLHttpRequest.HEADERS_RECEIVED;
				if (this.onreadystatechange) this.onreadystatechange();
				this.readyState = XMLHttpRequest.DONE;
				if (this.onreadystatechange) this.onreadystatechange();
			}
			get responseText() { return this._xhr.ResponseText; }
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

		StringBuilder script = new();
		script.AppendLine(InitialScriptText);
		script.AppendLine(ScriptText);
		script.AppendLine(ExecuteScriptText);

		TaskCompletionSource<object?> tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
		using CancellationTokenSource cts = new(TimeSpan.FromSeconds(60));
		cts.Token.Register(() => tcs.TrySetCanceled(cts.Token));
		Jint.Engine engine = new(options => options.CancellationToken(cts.Token));
		JintFunctions functions = new(engine, this, cts.Token);
		engine.SetValue("__setResult", new Action<object?>(result => tcs.TrySetResult(result)));
		engine.SetValue("__setError", new Action<string>(error => tcs.TrySetException(new Exception(error))));
		engine.SetValue("__functions", functions);
		try
		{
			engine.Execute(script.ToString());
		}
		catch (Exception ex)
		{
			tcs.TrySetException(ex);
		}
		try
		{
			var result = await tcs.Task;
			ResultText = result?.ToString() ?? "null";
		}
		catch (Exception ex)
		{
			ResultText = "Error: " + ex.Message;
		}
	}
}
