// MainPage.xaml.cs

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
}

