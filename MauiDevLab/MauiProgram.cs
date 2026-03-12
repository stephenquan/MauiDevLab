// MauiProgram.cs

using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Syncfusion.Maui.Toolkit.Hosting;

namespace MauiDevLab;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseSkiaSharp()
			.UseMauiCommunityToolkit()
			.UseMauiCommunityToolkitMarkup()
			.ConfigureSyncfusionToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
		;

		builder
			.RegisterDemo("VerticalStackLayout Demo (Slow)", nameof(VerticalStackLayoutDemo), typeof(VerticalStackLayoutDemo))
			.RegisterDemo("VerticalStackList Demo (Fast)", nameof(VerticalScrollStackDemo), typeof(VerticalScrollStackDemo))
			.RegisterDemo("Localize Demo", nameof(LocalizeDemo), typeof(LocalizeDemo))
			.RegisterDemo("Jint Demo", nameof(JintDemo), typeof(JintDemo))
			;

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
