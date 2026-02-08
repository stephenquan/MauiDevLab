// LocalizeBindingBase.cs

using System.Globalization;
using CommunityToolkit.Maui.Markup;

namespace MauiDevLab;

public class LocalizeBindingBase
{
	public static BindingBase Create<T>(Func<CultureInfo?, T> getKey, params object[] args)
	{
		ArgumentNullException.ThrowIfNull(getKey);
		var _args = args.ToArray();
		return BindingBase.Create<LocalizationManager, CultureInfo>(
			static lm => lm.Culture,
			BindingMode.OneWay,
			source: LocalizationManager.Current,
			converter: new FuncConverter<CultureInfo, T>(
				c =>
				{
					T result = getKey(c);
					if (typeof(T) == typeof(string)
						&& _args.Length > 0
						&& result is string format
						&& !string.IsNullOrEmpty(format))
					{
						result = (T)(object)string.Format(format, _args);
					}
					return result;
				}));
	}

	public static BindingBase Create(
		string path = ".",
		BindingMode mode = BindingMode.OneWay,
		IValueConverter? converter = null,
		object? converterParameter = null,
		string? stringFormat = null,
		object? source = null)
	{
		return new MultiBinding()
		{
			Bindings =
			[
				BindingBase.Create<LocalizationManager, CultureInfo>(
					static (lm) => lm.Culture,
					BindingMode.OneWay,
					source: LocalizationManager.Current),
				new Binding(path, mode, null, null, null, source)
			],
			Converter = new FuncMultiConverter<object?, object?[]>(
				v =>
				{
					var culture = (v[0] as CultureInfo) ?? LocalizationManager.Current.Culture;
					var result = v[1];
					if (converter is not null)
					{
						result = converter.Convert(result, null, converterParameter, culture);
					}
					return result;
				}
			)
		};
	}

	/*
	public static BindingBase Create<TSource, TProperty>(
		Func<TSource, TProperty> getter,
		BindingMode mode = BindingMode.OneWay,
		IValueConverter? converter = null,
		object? converterParameter = null,
		string? stringFormat = null,
		object? source = null)
	{
		return new MultiBinding()
		{
			Bindings =
			[
				BindingBase.Create<LocalizationManager, CultureInfo>(
					static (lm) => lm.Culture,
					BindingMode.OneWay,
					source: LocalizationManager.Current),
				//BindingBase.Create(getter, mode, null, null, null, source)
				//new CommunityToolkit.Maui.Markup.TypedBinding<TSource, TProperty(getter, null)>
			],
			Converter = new FuncMultiConverter<object?, object?[]>(
				v =>
				{
					var culture = (v[0] as CultureInfo) ?? LocalizationManager.Current.Culture;
					var result = v[1];
					if (converter is not null)
					{
						result = converter.Convert(result, null, converterParameter, culture);
					}
					return result;
				}
			)
		};
	}
	*/
}
