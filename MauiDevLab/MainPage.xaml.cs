// MainPage.xaml.cs

namespace MauiDevLab;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		BindingContext = this;
		InitializeComponent();
	}

	void ToggleLightDarkMode(object sender, EventArgs e)
	{
		ArgumentNullException.ThrowIfNull(App.Current);
		App.Current.UserAppTheme = App.Current.RequestedTheme == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark;
	}
}
