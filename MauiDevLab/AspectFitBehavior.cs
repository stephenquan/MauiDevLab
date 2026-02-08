// AspectFitBehavior.cs

using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;

namespace MauiDevLab;

public partial class AspectFitBehavior : Behavior<View>
{
	[BindableProperty] public partial double AspectRatio { get; set; } = 1.0;
	[BindableProperty] public partial View? ParentProxy { get; set; } = new ContentView();
	[BindableProperty] public partial double ParentWidth { get; set; } = 1.0;
	[BindableProperty] public partial Rect AspectSize { get; set; } = new Rect();
	protected override void OnAttachedTo(View bindable)
	{
		base.OnAttachedTo(bindable);
		this.SetBinding(
			ParentProxyProperty,
			BindingBase.Create<View, Element>(static v => v.Parent, BindingMode.OneWay, source: bindable,
				converter: new FuncConverter<Element, View?>(e => e as View)));
		this.SetBinding(
			AspectSizeProperty,
			new MultiBinding
			{
				Bindings =
				[
					BindingBase.Create<AspectFitBehavior, double>(static b => b.AspectRatio, BindingMode.OneWay, source: this),
					BindingBase.Create<AspectFitBehavior, double?>(static b => b.ParentProxy?.Width, BindingMode.OneWay, source: this),
					BindingBase.Create<AspectFitBehavior, double?>(static b => b.ParentProxy?.Height, BindingMode.OneWay, source: this),
				],
				Converter = new FuncMultiConverter<double, double?, double?, Rect>(
					((double a, double? w, double? h) v) =>
					{
						var r = v.w is null || v.h is null || v.a <= 0.0 || v.w <= 0 || v.h <= 0
							? Rect.Zero
							: v.h * v.a < v.w
							? new Rect(0, 0, (double)v.w, (double)v.h * v.a)
							: new Rect(0, 0, (double)v.w / v.a, (double)v.h);
						return r;
					})
			});

		bindable.SetBinding(
			VisualElement.WidthRequestProperty,
			BindingBase.Create<AspectFitBehavior, double>(static a => a.AspectSize.Width, BindingMode.OneWay, source: this));
		bindable.SetBinding(
			VisualElement.HeightRequestProperty,
			BindingBase.Create<AspectFitBehavior, double>(static a => a.AspectSize.Height, BindingMode.OneWay, source: this));
	}

	protected override void OnDetachingFrom(View bindable)
	{
		base.OnDetachingFrom(bindable);
		// Perform clean up
	}
}
