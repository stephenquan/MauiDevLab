// InputViewHelper.Android.cs

using Android.Text.Method;

namespace MauiDevLab;

public static partial class InputViewHelper
{
	public static partial void SetBorderThickness(this InputView inputView, int thickness)
	{
		if (inputView.Handler?.PlatformView is Microsoft.Maui.Platform.MauiAppCompatEditText editText)
		{
			editText.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
		}
	}

	public static partial void SetKeyListener(this InputView inputView, string keys)
	{
		if (inputView.Handler?.PlatformView is Microsoft.Maui.Platform.MauiAppCompatEditText editText)
		{
			editText.KeyListener = DigitsKeyListener.GetInstance(keys);
		}
	}

	public static partial void RegisterBeforeTextChangingHandler(this InputView inputView, EventHandler<BeforeTextChangingEventArgs> handler)
	{
		if (inputView.Handler?.PlatformView is Microsoft.Maui.Platform.MauiAppCompatEditText editText)
		{
		}
	}
}

