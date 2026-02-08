// VerticalScrollStack.cs

using System.Collections.ObjectModel;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;

namespace MauiDevLab;

public partial class VerticalScrollStack : ContentView
{
	[BindableProperty(PropertyChangedMethodName = nameof(UpdateVisibleItems))]
	public partial System.Collections.IEnumerable ItemsSource { get; set; }

	[BindableProperty]
	public partial Microsoft.Maui.Controls.DataTemplate ItemTemplate { get; set; }

	[BindableProperty(PropertyChangedMethodName = nameof(UpdateVisibleItems))]
	public partial int VisibleItemCount { get; set; } = 20;

	[BindableProperty]
	public partial double VisibleItemHeight { get; set; } = 44.0;

	public ObservableCollection<object?> VisibleItemsSource { get; } = [];

	[BindableProperty(PropertyChangedMethodName = nameof(UpdateVisibleItems))]
	public partial int ScrollPosition { get; set; } = 0;

	static void UpdateVisibleItems(BindableObject bindable, object oldValue, object newValue)
		=> ((VerticalScrollStack)bindable).UpdateVisibleItems();

	void UpdateVisibleItems()
	{
		if (ItemsSource is System.Collections.ICollection c)
		{
			int count = c.Count;
			int i = 0;
			int j = ScrollPosition;
			for (; i < VisibleItemCount && j < count; i++, j++)
			{
				var o = c.Cast<object?>().ElementAt(j);
				if (VisibleItemsSource.Count <= i)
				{
					VisibleItemsSource.Add(o);
				}
				else
				{
					VisibleItemsSource[i] = o;
				}
			}
			while (VisibleItemsSource.Count > i)
			{
				VisibleItemsSource.RemoveAt(VisibleItemsSource.Count - 1);
			}
		}
	}

	public VerticalScrollStack()
	{
		var innerLayout = new VerticalStackLayout();
		innerLayout.SetBinding(
			BindableLayout.ItemsSourceProperty,
			BindingBase.Create<VerticalScrollStack, ObservableCollection<object?>>(static cv => cv.VisibleItemsSource, BindingMode.OneWay, source: this));
		innerLayout.SetBinding(
			BindableLayout.ItemTemplateProperty,
			BindingBase.Create<VerticalScrollStack, DataTemplate>(static cv => cv.ItemTemplate, BindingMode.OneWay, source: this));
		this.SetBinding(
			VisibleItemCountProperty,
			new MultiBinding
			{
				Bindings =
				[
					BindingBase.Create<VisualElement, double>(static e => e.Height, BindingMode.OneWay, source: this),
					BindingBase.Create<VerticalScrollStack, double>(static l => l.VisibleItemHeight, BindingMode.OneWay, source: this),
				],
				Mode = BindingMode.OneWay,
				Converter = new FuncMultiConverter<double, double, int>
				(
					((double height, double visHeight) v) => v.visHeight >= 0 ? Math.Max((int)Math.Floor(v.height / v.visHeight), 1) : 1
				)
			});
		var sliderGrid = new Grid();
		Grid.SetColumn(sliderGrid, 1);
		var totalGrid = new Grid
		{
			IsClippedToBounds = true,
			ColumnDefinitions =
			[
				new ColumnDefinition(GridLength.Star),
				new ColumnDefinition(new GridLength(40))
			],
			Children = { innerLayout, sliderGrid }
		};
		var slider = new Slider();
		slider.SetBinding(
			WidthRequestProperty,
			BindingBase.Create<Grid, double>(static g => g.Height, BindingMode.OneWay, source: sliderGrid));
		slider.SetBinding(
			HeightRequestProperty,
			BindingBase.Create<Grid, double>(static g => g.Width, BindingMode.OneWay, source: sliderGrid));
		slider.Rotation = 90;
		slider.HorizontalOptions = LayoutOptions.Center;
		slider.VerticalOptions = LayoutOptions.Center;
		slider.Minimum = 0;
		slider.Maximum = 9980;
		slider.SetBinding(Slider.ValueProperty,
			BindingBase.Create<VerticalScrollStack, int>(static l => l.ScrollPosition, BindingMode.TwoWay, source: this));
		slider.SetBinding(
			Slider.MaximumProperty,
			new MultiBinding
			{
				Bindings =
				[
					new Binding("ItemsSource.Count", BindingMode.OneWay, source: this),
					BindingBase.Create<VerticalScrollStack, int>(static l => l.VisibleItemCount, BindingMode.OneWay, source: this)
				],
				Mode = BindingMode.OneWay,
				Converter = new FuncMultiConverter<int, int, int>(((int itemsCount, int visCount) v) => Math.Max(v.itemsCount - v.visCount / 2, 1))
			});
		sliderGrid.Children.Add(slider);
		Content = totalGrid;
	}

}
