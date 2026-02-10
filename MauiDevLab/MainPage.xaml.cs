// MainPage.xaml.cs

namespace MauiDevLab;

public partial class MainPage : ContentPage
{
	public MainViewModel VM { get; } = new();

	public MainPage()
	{
		BindingContext = VM;
		InitializeComponent();
	}
}

