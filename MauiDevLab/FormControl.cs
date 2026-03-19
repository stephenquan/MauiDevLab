// FormControl.cs

using BindablePropertyAttribute = CommunityToolkit.Maui.BindablePropertyAttribute;

namespace MauiDevLab;

public partial class FormControl : ContentView
{
	[BindableProperty(PropertyChangedMethodName = nameof(UpdateEntry))]
	public partial ExpressionNode? Node { get; set; }

	[BindableProperty(PropertyChangedMethodName = nameof(UpdateEntry))]
	public partial string QuestionType { get; set; } = string.Empty;

	static void UpdateEntry(BindableObject b, object? o, object? n)
		=> ((FormControl)b).UpdateEntry();

	void UpdateEntry()
	{
		if (Node is not null
			&& QuestionType is string questionType
			&& !string.IsNullOrEmpty(questionType))
		{
			switch (questionType)
			{
				case "FormInput":
					entry.Style = Application.Current?.Resources["FormInput"] as Style;
					entry.SetBinding(
						Entry.TextProperty,
						static (ExpressionNode? n) => n?.Value,
						BindingMode.TwoWay,
						source: Node);
					Content = entry;
					break;
				case "FormNote":
					entry.Style = Application.Current?.Resources["FormNote"] as Style;
					entry.SetBinding(
						Entry.TextProperty,
						static (ExpressionNode? n) => n?.Value,
						BindingMode.OneWay,
						source: Node);
					Content = entry;
					break;
			}
		}
		else
		{
			entry.RemoveBinding(Entry.TextProperty);

		}
	}

	Entry entry = new Entry();

	public FormControl()
	{
	}

	protected override void OnHandlerChanged()
	{
		base.OnHandlerChanged();
		if (Handler is null)
		{
			Node = null;
		}
	}

}
