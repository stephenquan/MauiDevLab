// JintFunctions.cs

using Jint;
using Jint.Native;

namespace MauiDevLab;

public class JintFunctions : CommonFunctions
{
	protected readonly Engine engine;

	public JintFunctions(Engine engine, Page page, CancellationToken ct) : base(page, ct)
	{
		ArgumentNullException.ThrowIfNull(engine, nameof(engine));
		this.engine = engine;
	}

	public XHR NewXHR() => new(engine, page);

	public JsValue FetchPromiseBridge(string url) => engine.ToPromise(FetchAsync, url, FinalizePromiseWithDispatcher);

	public void FinalizePromiseWithDispatcher(Action action)
		=> page.Dispatcher.Dispatch(action);
}
