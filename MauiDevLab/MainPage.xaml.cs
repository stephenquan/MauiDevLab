// MainPage.xaml.cs

namespace MauiDevLab;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	void InputView_Loaded(object sender, EventArgs e)
	{
		InputView inputView = (InputView)sender;
		inputView.SetBorderless();
	}
}
