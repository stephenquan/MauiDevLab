// LocalizeHelpers.cs

using System.Globalization;

namespace MauiDevLab;

public static class LocalizeHelpers
{
	public static T Localize<T, RT>(this T bindable, BindableProperty targetProperty, Func<CultureInfo?, RT> func, params object[] args) where T : BindableObject
	{
		bindable.SetBinding(targetProperty, LocalizeBindingBase.Create(func, args));
		return bindable;
	}

	public static T LocalizeRTL<T, RT>(this T bindable, BindableProperty targetProperty, Func<bool, RT> func, params object[] args) where T : BindableObject
	{
		bindable.SetBinding(
			targetProperty,
			LocalizeBindingBase.Create((c) => func(c is not null && c.TextInfo.IsRightToLeft), args));
		return bindable;
	}

	public static T LocalizeFlowDirection<T>(this T bindable) where T : VisualElement
	{
		return LocalizeRTL<T, FlowDirection>(bindable, VisualElement.FlowDirectionProperty, (isRTL) => isRTL ? FlowDirection.RightToLeft : FlowDirection.LeftToRight);
	}

	public static T LocalizeBind<T>(
		this T bindable,
		BindableProperty targetProperty,
		string path = ".",
		BindingMode mode = BindingMode.OneWay,
		IValueConverter? converter = null,
		object? converterParameter = null,
		string? stringFormat = null, object?
		source = null)
		where T : BindableObject
	{
		bindable.SetBinding(
			targetProperty,
			LocalizeBindingBase.Create(path, mode, converter, converterParameter, stringFormat, source));
		return bindable;
	}

}

