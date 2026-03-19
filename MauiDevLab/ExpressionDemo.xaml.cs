// ExpressionDemo.xaml.cs

using System.Diagnostics;

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

	//public static FuncConverter<string, Style?> StyleConverter = new(key => Application.Current?.Resources[key] as Style);

	//public static Style? FormInput = Application.Current?.Resources["FormInput"] as Style;
	//public static Style? FormNote = Application.Current?.Resources["FormNote"] as Style;

	//public static ControlTemplate FormInputTemplate = new ControlTemplate(
	//	() => new Entry { Style = FormInput }.Bind(Entry.TextProperty, "Node.Value", BindingMode.TwoWay));

	//public static ControlTemplate FormNodeTemplate = new ControlTemplate(
	//	() => new Entry { Style = FormNote }.Bind(Entry.TextProperty, "Node.Value", BindingMode.OneWay));

	public ExpressionDemo(ExpressionManager em)
	{
		EM = em;
		EM.SetInvokeOnUIThread(this.Dispatcher);

		BindingContext = this;
		InitializeComponent();

		Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(200), async () => await PopulateModelAsync());
	}

	async Task PopulateModelAsync()
	{
		Stopwatch sw = Stopwatch.StartNew();
		var controls = await Task.Run(() =>
		{
			int numExpressions = 33333;
			int numControls = numExpressions * 3;

			FormControlInfo[] controls = new FormControlInfo[numControls];

			Parallel.For(0, numExpressions, i =>
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
					Label = refA,
					Hint = "Enter value.",
					QuestionType = "FormInput",
				};

				controls[i * 3 + 1] = new FormControlInfo
				{
					Node = nodeB,
					Label = refB,
					Hint = $"Enter value.",
					QuestionType = "FormInput",
				};

				controls[i * 3 + 2] = new FormControlInfo
				{
					Node = nodeC,
					Label = refC,
					Hint = $"Result of for {expression}",
					QuestionType = "FormNote",
				};
			});

			return controls;
		});

		FormControls = controls.ToList();
		OnPropertyChanged(nameof(FormControls));

		sw.Stop();
		Debug.WriteLine($"Model population took {sw.Elapsed.TotalSeconds} seconds.");
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		if (cts is null)
		{
			cts = new CancellationTokenSource();
			EM.StartCalculationLoop(cts.Token);
		}
	}

	protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
	{
		base.OnNavigatedFrom(args);

		Dispatcher.Dispatch(async () =>
		{
			await EM.StopCalculationLoopAsync();
			cts?.Cancel();
			cts?.Dispose();
			cts = null;
			FormControls.Clear();
			EM.Clear();
		});
	}
}

