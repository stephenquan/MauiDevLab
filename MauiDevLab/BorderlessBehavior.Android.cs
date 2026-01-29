// BorderlessBehavior.Android.cs

namespace MauiDevLab;

public partial class BorderlessBehavior : PlatformBehavior<InputView>
{
	protected override void OnAttachedTo(InputView bindable, Android.Views.View platformView)
	{
		base.OnAttachedTo(bindable, platformView);
		platformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
	}
}
