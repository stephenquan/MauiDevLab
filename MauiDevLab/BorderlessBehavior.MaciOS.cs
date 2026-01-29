// BorderlessBehavior.MaciOS.cs

namespace MauiDevLab;

public partial class BorderlessBehavior : PlatformBehavior<InputView>
{
	protected override void OnAttachedTo(InputView bindable, UIKit.UIView platformView)
	{
		base.OnAttachedTo(bindable, platformView);
		switch (platformView)
		{
			case UIKit.UITextField textField:
				textField.Layer.BorderWidth = 0;
				break;
			case UIKit.UITextView textView:
				textView.Layer.BorderWidth = 0;
				break;
		}
	}
}
