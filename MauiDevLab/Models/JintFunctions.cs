// JintFunctions.cs

using Jint;
using Jint.Native;

namespace MauiDevLab;

public class JintFunctions : CommonFunctions
{
	protected readonly Engine engine;

	public JintFunctions(Engine engine, Page page, CancellationToken ct) : base(page, ct)
	{
		this.engine = engine;
	}

	public JsValue FetchPromiseBridge(string url) => engine.ToPromise(FetchAsync, url);
}
