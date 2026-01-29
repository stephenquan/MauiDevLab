// BorderlessBehavior.Windows.cs

namespace MauiDevLab;

public partial class BorderlessBehavior : PlatformBehavior<InputView>
{
	protected override void OnAttachedTo(InputView bindable, Microsoft.UI.Xaml.FrameworkElement platformView)
	{
		base.OnAttachedTo(bindable, platformView);
		if (platformView is Microsoft.UI.Xaml.Controls.TextBox textBox)
		{
			textBox.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
			textBox.Resources["TextControlBorderThemeThickness"] = new Microsoft.UI.Xaml.Thickness(0);
			textBox.Resources["TextControlBorderThemeThicknessFocused"] = new Microsoft.UI.Xaml.Thickness(0);
		}
	}
}
