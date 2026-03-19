// FormControl.cs

namespace MauiDevLab;

public partial class FormControl : ContentView
{
	Entry? entry;

	public FormControl()
	{
	}

	protected override void OnBindingContextChanged()
	{
		base.OnBindingContextChanged();

		Content = null;

		if (entry is not null)
		{
			entry.RemoveBinding(Entry.TextProperty);
			entry.Style = null;
			entry = null;
		}

		if (BindingContext is FormControlInfo info
			&& info.Node is not null
			&& info.QuestionType is string questionType
			&& !string.IsNullOrEmpty(questionType))
		{
			switch (questionType)
			{
				case "FormInput":
					entry = new();
					entry.Style = Application.Current?.Resources["FormInput"] as Style;
					entry.SetBinding(
						Entry.TextProperty,
						static (ExpressionNode? n) => n?.Value,
						BindingMode.TwoWay,
						source: info.Node);
					Content = entry;
					break;
				case "FormNote":
					entry = new();
					entry.Style = Application.Current?.Resources["FormNote"] as Style;
					entry.SetBinding(
						Entry.TextProperty,
						static (ExpressionNode? n) => n?.Value,
						BindingMode.OneWay,
						source: info.Node);
					Content = entry;
					break;
				default:
					break;
			}
		}
	}
}
