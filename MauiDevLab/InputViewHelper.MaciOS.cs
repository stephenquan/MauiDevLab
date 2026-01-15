// InputViewHelper.MaciOS.cs

using Foundation;
using UIKit;

namespace MauiDevLab;

public static partial class InputViewHelper
{
	public static partial void SetBorderThickness(this InputView inputView, int thickness)
	{
		if (inputView.Handler?.PlatformView is UIKit.UITextField textField)
		{
			textField.Layer.BorderWidth = 0;
		}

		if (inputView.Handler?.PlatformView is UIKit.UITextView textView)
		{
			textView.Layer.BorderWidth = 0;
		}
	}

	public static partial void SetKeyListener(this InputView inputView, string keys)
	{
	}

	public static partial void RegisterBeforeTextChangingHandler(this InputView inputView, EventHandler<BeforeTextChangingEventArgs> handler)
	{
		if (inputView.Handler?.PlatformView is UIKit.UITextField textField)
		{
			textField.Delegate = new BlockingTextFieldDelegate(inputView, textField, handler);
		}

		if (inputView.Handler?.PlatformView is UIKit.UITextView textView)
		{
		}
	}
}

class BlockingTextFieldDelegate : UIKit.UITextFieldDelegate
{
	InputView inputView;
	UIKit.UITextField textField;
	EventHandler<BeforeTextChangingEventArgs> handler;

	public BlockingTextFieldDelegate(InputView inputView, UIKit.UITextField textField, EventHandler<BeforeTextChangingEventArgs> handler)
	{
		this.inputView = inputView;
		this.textField = textField;
		this.handler = handler;
	}

	public override bool ShouldChangeCharacters(UITextField textField, NSRange range, string replacementString)
	{
		var oldText = textField.Text ?? string.Empty;
		var newText = oldText.Substring(0, (int)range.Location)
					  + replacementString
					  + oldText.Substring((int)(range.Location + range.Length));

		var arg = new BeforeTextChangingEventArgs(oldText, newText);
		handler.Invoke(textField, arg);
		if (arg.Cancel)
		{
			return false;
		}

		return true;
	}
}
