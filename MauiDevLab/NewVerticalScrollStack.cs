// NewVerticalScrollStack.cs

using System.Collections.ObjectModel;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;

namespace MauiDevLab;

public partial class NewVerticalScrollStack : ContentView
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
		=> ((NewVerticalScrollStack)bindable).UpdateVisibleItems();

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

	VerticalStackLayout vsl;
	ScrollView scrollView;
	Grid scrollViewGrid;
	public NewVerticalScrollStack()
	{
		Content = new Grid()
		{
			ColumnDefinitions = [new ColumnDefinition(GridLength.Star), new ColumnDefinition(20)],
			Children =
			{
				(vsl = new VerticalStackLayout
				{
					BackgroundColor = Colors.Orange,
					Children =
					{
						new Label { Text = "Hello" },
						new Label { Text = "World" }
					}
				}),
				(scrollView = new ScrollView()
				{
					Content = (scrollViewGrid = new Grid
					{
						HeightRequest = 1000
					})
				}).Column(1)
			}
		};
	}
}
