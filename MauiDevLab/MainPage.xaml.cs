// MainPage.xaml.cs

//#define LEAK_DIAGNOSTICS

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

