// MainViewModel.cs

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiDevLab;

public partial class MyItem : ObservableObject
{
	[ObservableProperty]
	public partial int ItemId { get; set; } = 0;
	[ObservableProperty]
	public partial string TextValue { get; set; } = string.Empty;
	public static MyItem Zero = new();
}

public partial class MainViewModel : ObservableObject
{
	public ObservableCollection<MyItem> MyItems { get; } = [];

	public MainViewModel()
	{
		for (int i = 0; i <= 500000; i++)
		{
			MyItems.Add(new MyItem() { ItemId = i });
		}
	}
}
