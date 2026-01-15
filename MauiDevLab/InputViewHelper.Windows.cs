// InputViewHelper.Windows.cs

namespace MauiDevLab;

public static partial class InputViewHelper
{
	public static partial void SetBorderThickness(this InputView inputView, int thickness)
	{
		if (inputView.Handler?.PlatformView is Microsoft.UI.Xaml.Controls.TextBox textBox)
		{
			textBox.BorderThickness = new Microsoft.UI.Xaml.Thickness(thickness);
		}
	}

	public static partial void SetKeyListener(this InputView inputView, string keys)
	{
	}

	public static partial void RegisterBeforeTextChangingHandler(this InputView inputView, EventHandler<BeforeTextChangingEventArgs> handler)
	{
		if (inputView.Handler?.PlatformView is Microsoft.UI.Xaml.Controls.TextBox textBox)
		{
			textBox.BeforeTextChanging += (s, e) =>
			{
				var args = new BeforeTextChangingEventArgs(textBox?.Text.ToString() ?? string.Empty, e.NewText);
				handler.Invoke(inputView, args);
				if (args.Cancel)
				{
					e.Cancel = true;
				}
			};
		}
	}
}
