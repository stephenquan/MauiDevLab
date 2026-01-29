// InputMaskBehavior.shared.cs

using SQuan.Helpers.Maui.Mvvm;

namespace MauiDevLab;

public partial class InputMaskBehavior : PlatformBehavior<InputView>
{
	[BindableProperty]
	public partial string? Regex { get; set; }

	[BindableProperty]
	public partial string? Keys { get; set; }
}
