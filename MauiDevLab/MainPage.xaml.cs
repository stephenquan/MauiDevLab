// MainPage.xaml.cs

using System.Text.RegularExpressions;

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
		inputView.SetBorderThickness(0);
		inputView.SetKeyListener("0123456789"); // Android only
		inputView.RegisterBeforeTextChangingHandler(Numeric_BeforeTextChangingHandler); // Windows/iOS
	}

	void Numeric_BeforeTextChangingHandler(object? sender, BeforeTextChangingEventArgs e)
	{
		if (Regex.Match(e.NewTextValue, "^[0-9]*$").Success == false)
		{
			e.Cancel = true;
		}
	}
}
