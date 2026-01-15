// InputViewHelper.Windows.cs

namespace MauiDevLab;

public static partial class InputViewHelper
{
	public static partial void SetBorderless(this InputView inputView)
	{
		if (inputView.Handler?.PlatformView is Microsoft.UI.Xaml.Controls.TextBox textBox)
		{
			textBox.FontWeight = Microsoft.UI.Text.FontWeights.Thin;
			textBox.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
		}
	}
}
