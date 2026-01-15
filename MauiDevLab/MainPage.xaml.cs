// MainPage.xaml.cs

namespace MauiDevLab;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	void Editor_Loaded(object sender, EventArgs e)
	{
		InputView inputView = (InputView)sender;
		inputView.SetBorderless();
	}

	void Editor_Focused(object sender, FocusEventArgs e)
	{
		InputView inputView = (InputView)sender;
		inputView.Dispatcher.Dispatch(() => inputView.SetBorderless());
	}
}
