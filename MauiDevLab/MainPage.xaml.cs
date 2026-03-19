// MainPage.xaml.cs

// Uncomment the following line to enable leak diagnostics on the MainPage. This will force a GC collection when navigating to the page, which can help identify if there are any leaks related to the page or its resources. Note that this should only be used in debug builds and may impact performance due to the forced garbage collection.
// #define LEAK_DIAGNOSTICS

using System.Reflection;

namespace MauiDevLab;

public partial class MainPage : ContentPage
{
	public List<AssemblyMetadataAttribute> AppVersionInfos { get; }
		= typeof(MainPage).Assembly
			.GetCustomAttributes<AssemblyMetadataAttribute>()
			.ToList();

	public MainPage()
	{
		BindingContext = this;
		InitializeComponent();
	}

	async void OnStartDemo(object sender, EventArgs e)
		=> await Shell.Current.GoToAsync((((Button)sender).Text.GetDemoRoute()));

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);
#if DEBUG && LEAK_DIAGNOSTICS
		_ = Dispatcher.Dispatch(async () =>
		{
			await Task.Delay(1000);
			GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);
			GC.WaitForPendingFinalizers();
			GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);
		});
#endif
	}
}

