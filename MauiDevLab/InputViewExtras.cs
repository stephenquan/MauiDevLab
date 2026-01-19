// InputViewExtras.cs

namespace MauiDevLab;


public partial class InputViewExtras
{
	#region BorderThickness Attached Property
	public static readonly BindableProperty BorderThicknessProperty = BindableProperty.CreateAttached("BorderThickness", typeof(int), typeof(InputViewExtras), 1, BindingMode.OneWay,
		coerceValue: (b, v) => InvokeActionOnHelper(b, v, (platform) => platform.BorderThickness = (int)v));
	public static int GetBorderThickness(BindableObject view) => (int)view.GetValue(BorderThicknessProperty);
	public static void SetBorderThickness(BindableObject view, int value) => view.SetValue(BorderThicknessProperty, value);
	#endregion

	#region MaskType Attached Property
	public static readonly BindableProperty MaskTypeProperty = BindableProperty.CreateAttached("MaskType", typeof(InputViewMaskKind), typeof(InputViewExtras), InputViewMaskKind.None, BindingMode.OneWay,
		coerceValue: (b, v) => InvokeActionOnHelper(b, v, (platform) => platform.MaskType = (InputViewMaskKind)v));
	public static InputViewMaskKind GetMaskType(BindableObject view) => (InputViewMaskKind)view.GetValue(MaskTypeProperty);
	public static void SetMaskType(BindableObject view, InputViewMaskKind value) => view.SetValue(MaskTypeProperty, value);
	#endregion

	#region InputViewHelper Attached Property
	public static readonly BindableProperty InputViewHelperProperty = BindableProperty.CreateAttached("InputViewHelper", typeof(InputViewHelper), typeof(InputViewExtras), null, BindingMode.OneWay);
	public static InputViewHelper? GetInputViewHelper(BindableObject view) => (InputViewHelper?)view.GetValue(InputViewHelperProperty);
	public static void SetInputViewHelper(BindableObject view, InputViewHelper? value) => view.SetValue(InputViewHelperProperty, value);
	#endregion

	#region InvokeActionOnHelper Method
	static object? InvokeActionOnHelper(BindableObject view, object? value, Action<InputViewHelper> action)
	{
		if (view is InputView inputView)
		{
			InputViewHelper? platform = GetInputViewHelper(view);
			if (platform is null)
			{
				platform = new InputViewHelper(inputView);
				SetInputViewHelper(view, platform);
			}
			action(platform);
		}
		return value;
	}
	#endregion
}
