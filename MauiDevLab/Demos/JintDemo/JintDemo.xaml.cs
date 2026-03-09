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
			var json = await FetchAsync("https://www.arcgis.com/sharing/rest/info?f=pjson");
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
		} = __functions;

		const console = {
			log: (...args) => TraceWriteLine(...args),
			error: (...args) => TraceWriteLine("ERROR:", ...args)
		};
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
		var engineContext = SynchronizationContext.Current;
		ArgumentNullException.ThrowIfNull(engineContext, nameof(engineContext));
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
