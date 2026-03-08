// LocalizeDemo.xaml.cs

using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace MauiDevLab;

public partial class LocalizeDemo : ContentPage
{
	public ObservableCollection<MyItem> LocalizeItems { get; } = [];
	static String[] names = new[] { "Harry", "Sally", "Tom", "Jane", "Bill", "Susan" };
	public int MyItemCount => MyItem.InstanceCount;
	public int LocalizeExtensionCount => LocalizeExtension.InstanceCount;
	IDispatcherTimer? timer;

	public LocalizeDemo()
	{
		BindingContext = this;
		InitializeComponent();
	}

	[GeneratedRegex(@"^Add (\d+) Items$")]
	private static partial Regex AddItemsRegex();

	void OnAddItems(object sender, EventArgs e)
	{
		if (((Button)sender).Text is string buttonText
			&& AddItemsRegex().Match(buttonText) is Match match
			&& match.Success
			&& int.TryParse(match.Groups[1].Value, out int more))
		{
			for (int i = 0; i < more; i++)
			{
				LocalizeItems.Add(new MyItem()
				{
					ItemId = LocalizeItems.Count + 1,
					TextValue = names[Random.Shared.Next(names.Length)]
				});
			}
		}
	}

	void OnClearItems(object sender, EventArgs e)
	{
		LocalizeItems.Clear();
	}

	void OnGC(object sender, EventArgs e)
	{
		GC.Collect();
	}

	void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
	{
		timer = Dispatcher.CreateTimer();
		timer.Interval = TimeSpan.FromSeconds(1);
		timer.Tick += Timer_Tick;
		timer.Start();
	}

	void ContentPage_NavigatedFrom(object sender, NavigatedFromEventArgs e)
	{
		if (timer is not null)
		{
			timer.Stop();
			timer.Tick -= Timer_Tick;
			timer = null;
		}
	}

	void Timer_Tick(object? sender, EventArgs e)
	{
		OnPropertyChanged(nameof(MyItemCount));
		OnPropertyChanged(nameof(LocalizeExtensionCount));
	}
}
