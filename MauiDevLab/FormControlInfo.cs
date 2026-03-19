// FormControlInfo.cs

using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiDevLab;

public partial class FormControlInfo : ObservableObject
{
	[ObservableProperty]
	public partial ExpressionNode? Node { get; set; }

	[ObservableProperty]
	public partial string Label { get; set; } = string.Empty;

	[ObservableProperty]
	public partial string Hint { get; set; } = string.Empty;

	[ObservableProperty]
	public partial string QuestionType { get; set; } = string.Empty;
}
