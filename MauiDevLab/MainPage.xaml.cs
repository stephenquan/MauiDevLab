// MainPage.xaml.cs

using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Layouts;

namespace MauiDevLab;

public partial class Collapser : ContentView
{
	[BindableProperty]
	public partial string LabelText { get; set; } = string.Empty;
	public Collapser()
	{
		ControlTemplate = new(
			() => new VerticalStackLayout
			{
				new HorizontalStackLayout { new Label().Bind(Label.TextProperty, (Collapser c) => c.LabelText, source: this) },
				new ContentPresenter()
			});
	}
}

public partial class MainPage : ContentPage
{
	public MainViewModel VM { get; } = new();

	public MainPage()
	{
		BindingContext = VM;
		InitializeComponent();
	}

	void ScrollView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		if (e.PropertyName == ScrollView.ScrollYProperty.PropertyName)
		{
			System.Diagnostics.Trace.WriteLine($"ScrollView property changed: {e.PropertyName} {((ScrollView)sender).ScrollY}");
		}
		else
		{
			System.Diagnostics.Trace.WriteLine($"ScrollView property changed: {e.PropertyName}");
		}
	}

	void ScrollView_Scrolled(object sender, ScrolledEventArgs e)
	{
		System.Diagnostics.Trace.WriteLine($"ScrollView scrolled: {e.ScrollX}, {e.ScrollY}");
	}

	async void OnStartDemo(object sender, EventArgs e)
		=> await Shell.Current.GoToAsync((((Button)sender).Text.GetDemoRoute()));
}


public class FastLayout : Layout
{
	protected override ILayoutManager CreateLayoutManager()
		=> new FastLayoutManager(this);
}

class FastLayoutManager : ILayoutManager
{
	readonly FastLayout layout;

	public FastLayoutManager(FastLayout layout)
		=> this.layout = layout;

	public Size Measure(double width, double height)
	{
		// You control measurement here
		//return new Size(width, height);
		return new Size(200, 20);
	}

	public Size ArrangeChildren(Rect bounds)
	{

		double y = bounds.Y;

		foreach (var child in layout.Children)
		{
			var h = 40; //  child.DesiredSize.Height;

			// ✅ THIS LINE IS REQUIRED
			child.Arrange(new Rect(bounds.X, y, bounds.Width, h));

			y += h;
		}


		// You control arrangement here
		return bounds.Size;
	}
}


