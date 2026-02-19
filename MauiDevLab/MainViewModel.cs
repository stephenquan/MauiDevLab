// MainViewModel.cs

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Jint;

namespace MauiDevLab;

public partial class MyItem : ObservableObject
{
	[ObservableProperty] public partial int ItemId { get; set; } = 0;
	[ObservableProperty] public partial string TextValue { get; set; } = string.Empty;
	[ObservableProperty] public partial double TY { get; set; } = 0.0;
	[ObservableProperty] public partial double H { get; set; } = 0.0;
	public static MyItem Zero = new();
}

public partial class MainViewModel : ObservableObject
{
	[ObservableProperty] public partial int Count { get; set; } = 0;
	[ObservableProperty] public partial double TotalHeight { get; set; } = 0.0;

	public ObservableCollection<MyItem> MyItems { get; } = [];

	[ObservableProperty] public partial string ButtonText { get; set; } = "Click me";

	public MainViewModel()
	{
		double TY = 0.0;
		for (int i = 0; i <= 1000; i++)
		{
			double H = 40.0;
			MyItems.Add(new MyItem() { ItemId = i, TY = TY, H = H, TextValue = $"Item #{i}" });
			TY += H;
			TotalHeight += H;
		}
	}

	[RelayCommand]
	async Task Clicked()
	{
		Count++;
		//dispatcher.DispatchAsync(async () => await RunAsync());
		//await RunAsync();
	}

	async Task RunAsync()
	{
		try
		{
			ButtonText = "Calculating...";
			await Task.Delay(50);
			Engine jintEngine = new();
			jintEngine.SetValue("add_async", jintEngine.ToJsPromiseFunc<int, int, int>(add_async));
			jintEngine.SetValue("op_async", jintEngine.ToJsPromiseFunc<string, object?, object?, object?>(op_async));
			jintEngine.SetValue("concat_async", jintEngine.ToJsPromiseFunc<object?[], string>(concat_async));
			TaskCompletionSource<object?> tcs = new();
			jintEngine.SetValue("set_result", new Action<object?>(v => tcs.SetResult(v)));
			jintEngine.Execute(
				$$"""
                (async () =>
                {
                    var s;
                    s= [ ];
                    s.push("Cat: ");
                    s.push(12);
                    s.push(6);
                    s.push(await op_async("div", ...s.splice(-2,2)));
                    s.push(5);
                    s.push(6);
                    s.push(await op_async("*", ...s.splice(-2,2)));
                    s.push(await op_async("+", ...s.splice(-2,2)));
                    s.push(await concat_async(s.splice(-2,2)));
                    set_result(s.pop());
                })();
                """);
			var result = await tcs.Task;
			ButtonText = result?.ToString() ?? string.Empty;
		}
		catch (Exception err)
		{
			ButtonText = "Exception: " + err.Message;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <returns></returns>
	public static async Task<int> add_async(int a, int b)
	{
		await Task.Delay(250);
		return a + b;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="op"></param>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <returns></returns>
	public static async Task<object?> op_async(string op, object? a, object? b)
	{
		await Task.Delay(250);

		if (string.IsNullOrEmpty(op) || a is null || b is null)
		{
			return null;
		}

		if (ConvertToDouble(a, out var dblA) && ConvertToDouble(b, out var dblB))
		{
			return op switch
			{
				"+" => dblA + dblB,
				"-" => dblA - dblB,
				"*" => dblA * dblB,
				"div" => dblA / dblB,
				"mod" => dblA % dblB,
				">=" => dblA >= dblB,
				">" => dblA > dblB,
				"<=" => dblA <= dblB,
				"<" => dblA < dblB,
				_ => null
			};
		}


		return null;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="value"></param>
	/// <param name="dblValue"></param>
	/// <returns></returns>
	public static bool ConvertToDouble(object? value, out double dblValue)
	{
		dblValue = double.NaN;
		if (value is double _dblValue)
		{
			dblValue = _dblValue;
			return true;
		}
		if (value is int intValue)
		{
			dblValue = intValue;
			return true;
		}
		return false;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="args"></param>
	/// <returns></returns>
	public static async Task<string> concat_async(params object?[] args)
	{
		//await Task.Delay(250);
		return string.Concat(args);
	}
}
