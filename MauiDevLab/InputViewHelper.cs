// InputViewHelper.cs

namespace MauiDevLab;

public static partial class InputViewHelper
{
	public static partial void SetBorderThickness(this InputView inputView, int thickness);
	public static partial void SetKeyListener(this InputView inputView, string keys);
	public static partial void RegisterBeforeTextChangingHandler(this InputView inputView, EventHandler<BeforeTextChangingEventArgs> handler);
}

public class BeforeTextChangingEventArgs : TextChangedEventArgs
{
	public bool Cancel { get; set; } = false;

	public BeforeTextChangingEventArgs(string oldTextValue, string newTextValue, bool cancel = false) : base(oldTextValue, newTextValue)
	{
		Cancel = cancel;
	}
}
