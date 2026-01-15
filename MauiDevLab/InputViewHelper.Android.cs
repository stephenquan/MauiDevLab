// InputViewHelper.Android.cs

namespace MauiDevLab;

public static partial class InputViewHelper
{
	public static partial void SetBorderless(this InputView inputView)
	{
		if (inputView.Handler?.PlatformView is Microsoft.Maui.Platform.MauiAppCompatEditText editText)
		{
			editText.SetBackgroundColor(Android.Graphics.Color.Transparent);
			editText.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
		}
	}
}
