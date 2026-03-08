// MyItem.cs

using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiDevLab;

public partial class MyItem : ObservableObject
{
	[ObservableProperty] public partial int ItemId { get; set; } = 0;
	[ObservableProperty] public partial string TextValue { get; set; } = string.Empty;

	public static int InstanceCount = 0;

	public MyItem()
	{
		_ = Interlocked.Increment(ref InstanceCount);
	}

	~MyItem()
	{
		_ = Interlocked.Decrement(ref InstanceCount);
	}
}
