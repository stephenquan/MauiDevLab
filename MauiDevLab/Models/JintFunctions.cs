// JintFunctions.cs

using Jint;
using Jint.Native;

namespace MauiDevLab;

public class JintFunctions : CommonFunctions
{
	protected readonly Engine engine;
	protected readonly SynchronizationContext engineContext;

	public JintFunctions(Engine engine, SynchronizationContext engineContext, Page page, CancellationToken ct) : base(page, ct)
	{
		ArgumentNullException.ThrowIfNull(engine, nameof(engine));
		ArgumentNullException.ThrowIfNull(engineContext, nameof(engineContext));
		this.engine = engine;
		this.engineContext = engineContext;
	}

	public JsValue FetchPromiseBridge(string url) => engine.ToPromise(FetchAsync, url, engineContext);
}
