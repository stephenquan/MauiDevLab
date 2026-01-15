// InputViewHelper.MaciOS.cs

namespace MauiDevLab;

public static partial class InputViewHelper
{
	public static partial void SetBorderless(this InputView inputView)
	{
		if (inputView.Handler?.PlatformView is UIKit.UITextField textField)
		{
			textField.BackgroundColor = UIKit.UIColor.Clear;
			textField.Layer.BorderWidth = 0;
			textField.BorderStyle = UIKit.UITextBorderStyle.None;
		}

		if (inputView.Handler?.PlatformView is UIKit.UITextView textView)
		{
			textView.BackgroundColor = UIKit.UIColor.Clear;
			textView.Layer.BorderWidth = 0;
		}
	}
}
