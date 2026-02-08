// AspectView.cs

using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;

namespace MauiDevLab;

public partial class AspectView : ContentView
{
	[BindableProperty(PropertyChangedMethodName = nameof(RaiseContentSizeChanged))]
	public partial Aspect Aspect { get; set; } = Aspect.AspectFit;
	[BindableProperty(PropertyChangedMethodName = nameof(RaiseContentSizeChanged))]
	public partial double AspectRatio { get; set; } = 1.0;
	[BindableProperty]
	public partial View ParentProxy { get; set; } = new ContentView();
	public Rect ContentSize
		=> (Width <= 0 || Height <= 0 || AspectRatio <= 0)
		? Rect.Zero
		: Aspect switch
		{
			Aspect.AspectFit => (Height * AspectRatio > Width)
				? new Rect(0, 0, Width, Width / AspectRatio)
				: new Rect(0, 0, Height * AspectRatio, Height),
			Aspect.AspectFill => (Height * AspectRatio > Width)
				? new Rect(0, 0, Height * AspectRatio, Height)
				: new Rect(0, 0, Width, Width / AspectRatio),
			_ => new Rect(0, 0, Width, Height)
		};
	static void RaiseContentSizeChanged(BindableObject bindable, object oldValue, object newValue)
		=> ((AspectView)bindable).OnPropertyChanged(nameof(ContentSize));
	public AspectView()
	{
		this.SetBinding(
			ParentProxyProperty,
			BindingBase.Create<ContentView, Element>(static v => v.Parent, BindingMode.OneWay, source: this,
				converter: new FuncConverter<Element, View?>(e => e as View)));
		ControlTemplate = new(() =>
		{
			var cp = new ContentPresenter();
			cp.SetBinding(WidthRequestProperty, static (AspectView a) => a.ContentSize.Width, source: this);
			cp.SetBinding(HeightRequestProperty, static (AspectView a) => a.ContentSize.Height, source: this);
			return cp;
		});
		SizeChanged += (s, e) => OnPropertyChanged(nameof(ContentSize));
	}
}
