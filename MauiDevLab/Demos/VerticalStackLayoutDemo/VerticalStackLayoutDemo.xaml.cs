// VerticalStackLayoutDemo.xaml.cs

using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace MauiDevLab;

public partial class VerticalStackLayoutDemo : ContentPage
{
	public ObservableCollection<MyItem> SlowItems { get; } = [];

	public VerticalStackLayoutDemo()
	{
		BindingContext = this;
		InitializeComponent();
	}

	[GeneratedRegex($"^Add (\\d+)$")]
	private static partial Regex AddItemsRegex();

	void OnAddMany(object sender, EventArgs e)
	{
		if (((Button)sender).Text is string buttonText
			&& AddItemsRegex().Match(buttonText) is Match match
			&& match.Success
			&& match.Groups[1].Value is string moreString
			&& int.TryParse(moreString, out int more))
		{
			for (int i = 0; i < more; i++)
			{
				SlowItems.Add(new MyItem() { ItemId = SlowItems.Count, TextValue = $"Item #{SlowItems.Count}" });
			}
		}
	}
}
