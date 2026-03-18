// ExpressionDemo.xaml.cs

using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiDevLab;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
	"Design",
	"CA1001:Types that own disposable fields should be disposable",
	Justification = "CTS is created and cancelled within navigation lifecycle; no disposal required.")]
public partial class ExpressionDemo : ContentPage
{
	public ExpressionManager EM { get; }
	CancellationTokenSource? cts;
	public List<FormControlInfo> FormControls { get; set; } = [];

	public static FuncConverter<string, Style?> StyleConverter = new(key => Application.Current?.Resources[key] as Style);

	public static Style? FormInput = Application.Current?.Resources["FormInput"] as Style;
	public static Style? FormNote = Application.Current?.Resources["FormNote"] as Style;

	public static ControlTemplate FormInputTemplate = new ControlTemplate(
		() => new Entry { Style = FormInput }.Bind(Entry.TextProperty, "Node.Value", BindingMode.TwoWay));

	public static ControlTemplate FormNodeTemplate = new ControlTemplate(
		() => new Entry { Style = FormNote }.Bind(Entry.TextProperty, "Node.Value", BindingMode.OneWay));

	public ExpressionDemo(ExpressionManager em)
	{
		EM = em;
		EM.SetInvokeOnUIThread(this.Dispatcher);

		BindingContext = this;
		InitializeComponent();

		_ = PopulateModel();
	}

	async Task PopulateModel()
	{
		var controls = await Task.Run(() =>
		{
			FormControlInfo[] controls = new FormControlInfo[99999];

			Parallel.For(0, 33333, i =>
			{
				string refA = $"/survey/x{i * 3 + 1}";
				string refB = $"/survey/x{i * 3 + 2}";
				string refC = $"/string/x{i * 3 + 3}";

				string expression = Random.Shared.Next(0, 10) switch
				{
					0 => $"{refA} + {refB}",
					1 => $"{refA} - {refB}",
					2 => $"{refA} * {refB}",
					3 => $"{refA} div {refB}",
					4 => $"sqrt({refA} * {refA} + {refB} * {refB})",
					5 => $"if ({refA} > {refB}, {refA}, {refB})",
					6 => $"if ({refA} < {refB}, {refA}, {refB})",
					7 => $"concat({refA}, 1001, {refB})",
					8 => $"sqrt(pow({refA}, 2) + pow({refB}, 2))",
					9 => $"{refA} mod {refB}",
					_ => $"{refA} + {refB}",
				};

				var nodeA = EM.SetValue<double>(refA, Random.Shared.Next(0, 100));
				var nodeB = EM.SetValue<double>(refB, Random.Shared.Next(0, 100));
				var nodeC = EM.SetExpression<double>(refC, expression);

				controls[i * 3 + 0] = new FormControlInfo
				{
					Node = nodeA,
					NodeRef = refA,
					Label = refA,
					Hint = "Enter value.",
					Style = "FormInput",
					Mode = BindingMode.TwoWay,
					ControlTemplate = FormInputTemplate,
				};

				controls[i * 3 + 1] = new FormControlInfo
				{
					Node = nodeB,
					NodeRef = refB,
					Label = refB,
					Hint = $"Enter value.",
					Style = "FormInput",
					Mode = BindingMode.TwoWay,
					ControlTemplate = FormInputTemplate,
				};

				controls[i * 3 + 2] = new FormControlInfo
				{
					Node = nodeC,
					NodeRef = refC,
					Label = refC,
					Hint = $"Result of for {expression}",
					Style = "FormNote",
					ControlTemplate = FormNodeTemplate,
				};
			});

			return controls;
		});

		FormControls = controls.ToList();
		OnPropertyChanged(nameof(FormControls));
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		if (cts is null)
		{
			cts = new CancellationTokenSource();
			EM.StartCalculationLoop(cts.Token);
		}
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();

		EM.StopCalculationLoop().GetAwaiter().GetResult();
		cts?.Cancel();
		cts?.Dispose();
		cts = null;
		FormControls.Clear();
		EM.Clear();
	}
}

public partial class FormControlInfo : ObservableObject
{
	[ObservableProperty]
	public partial string NodeRef { get; set; } = "NODEREF";

	[ObservableProperty]
	public partial string Label { get; set; } = "LABEL";

	[ObservableProperty]
	public partial string Hint { get; set; } = "HINT";

	[ObservableProperty]
	public partial BindingMode Mode { get; set; } = BindingMode.OneWay;

	[ObservableProperty]
	public partial object? Value { get; set; }

	[ObservableProperty]
	public partial BindingBase Binding { get; set; }

	[ObservableProperty]
	public partial ControlTemplate? ControlTemplate { get; set; }

	[ObservableProperty]
	public partial string Style { get; set; } = string.Empty;

	[ObservableProperty]
	public partial ExpressionNode? Node { get; set; }
}
