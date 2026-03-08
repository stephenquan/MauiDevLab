// JintFunctions.cs

using Jint;
using Jint.Native;

namespace MauiDevLab;

public class JintFunctions : CommonFunctions
{
	protected readonly Engine engine;

	public JintFunctions(Page page, Engine engine) : base(page)
	{
		this.engine = engine;
	}

	public JsValue FetchPromiseBridge(string url)
		=> engine.ToPromise(FetchAsync, url, FinalizePromiseWithDispatcher);

	void FinalizePromiseWithDispatcher(Action finalizeAction)
		=> page.Dispatcher.Dispatch(finalizeAction);
}
