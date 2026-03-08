// LocalizeExtension.cs

using System.Globalization;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Converters;
using MauiDevLab.Resources.Strings;

namespace MauiDevLab;

[ContentProperty(nameof(Key))]
[RequireService([typeof(IReferenceProvider), typeof(IProvideValueTarget)])]
public partial class LocalizeExtension : BindableObject, IMarkupExtension<BindingBase>
{
	public static int InstanceCount = 0;

	[BindableProperty(CoerceValueMethodName = nameof(CoerceAndInvokeResultChanged))]
	public partial string Key { get; set; } = string.Empty;

	[BindableProperty(CoerceValueMethodName = nameof(CoerceAndInvokeResultChanged))]
	public partial object? X0 { get; set; }

	[BindableProperty(CoerceValueMethodName = nameof(CoerceAndInvokeResultChanged))]
	public partial object? X1 { get; set; }

	[BindableProperty(CoerceValueMethodName = nameof(CoerceHasWindow))]
	public partial bool InternalHasWindow { get; set; }

	bool subscribed = false;

	static object CoerceHasWindow(BindableObject b, object v)
		=> ((LocalizeExtension)b).CoerceHasWindow((bool)v);

	static object CoerceAndInvokeResultChanged(BindableObject b, object v)
	{
		var ext = (LocalizeExtension)b;
		ext.OnPropertyChanged(nameof(Result));
		return v;
	}

	bool CoerceHasWindow(bool value)
	{
		if (value)
		{
			if (!subscribed)
			{
				LocalizationManager.Current.CultureChanged += OnCultureChanged;
				subscribed = true;
				OnPropertyChanged(nameof(Result));
			}
		}
		else
		{
			if (subscribed)
			{
				LocalizationManager.Current.CultureChanged -= OnCultureChanged;
				subscribed = false;
			}
		}
		return value;
	}

	public string Result
	{
		get
		{
			if (string.IsNullOrEmpty(Key))
			{
				return string.Empty;
			}

			if (!subscribed)
			{
				return Key;
			}

			var localized = AppStrings.ResourceManager.GetString(Key, LocalizationManager.Current.Culture) ?? Key;
			if (string.IsNullOrEmpty(localized))
			{
				return Key;
			}

			return string.Format(localized, X0, X1);
		}
	}

	public LocalizeExtension()
	{
		InstanceCount++;
	}

	~LocalizeExtension()
	{
		InstanceCount--;
	}

	public object ProvideValue(IServiceProvider serviceProvider)
	=> (this as IMarkupExtension<BindingBase>).ProvideValue(serviceProvider);

	BindingBase IMarkupExtension<BindingBase>.ProvideValue(IServiceProvider serviceProvider)
	{
		if (serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget provideValueTarget
			&& provideValueTarget.TargetObject is VisualElement targetObject)
		{
			this.SetBinding(InternalHasWindowProperty,
				new MultiBinding()
				{
					Bindings =
					{
						/*
						BindingBase.Create<VisualElement, Window?>(
							static e => e.Window, BindingMode.OneWay, converter: new IsNotNullConverter(),
							source: targetObject),
						*/
						new Binding("Window", BindingMode.OneWay, converter: new IsNotNullConverter(), source: targetObject),
						BindingBase.Create<VisualElement, bool>(
							static e=> e.IsVisible, BindingMode.OneWay, source: targetObject)
					},
					Converter = new VariableMultiValueConverter()
					{
						ConditionType = MultiBindingCondition.All
					}
				});
			this.SetBinding(BindableObject.BindingContextProperty, static (BindableObject t) => t.BindingContext, BindingMode.OneWay, source: targetObject);
		}
		return BindingBase.Create<LocalizeExtension, string>(static e => e.Result, BindingMode.OneWay, source: this);
	}

	void OnCultureChanged(object? sender, CultureInfo e)
		=> OnPropertyChanged(nameof(Result));
}
