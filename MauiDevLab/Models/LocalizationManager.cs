// LocalizationManager.cs

using System.ComponentModel;
using System.Globalization;
using CommunityToolkit.Mvvm.Messaging;

namespace MauiDevLab;

public partial class LocalizationManager : INotifyPropertyChanged
{
	public static LocalizationManager Current { get; } = new LocalizationManager();

	public CultureInfo Culture
	{
		get => CultureInfo.CurrentUICulture;
		set
		{
			ArgumentNullException.ThrowIfNull(value, nameof(value));
			if (value.Name == CultureInfo.CurrentUICulture.Name)
			{
				return;
			}
			CultureInfo.CurrentUICulture = value;
			WeakReferenceMessenger.Default.Send(new LocalizeCultureMessage(value));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Culture)));
			CultureChanged?.Invoke(this, value);
		}
	}

	public EventHandler<CultureInfo>? CultureChanged;

	public event PropertyChangedEventHandler? PropertyChanged;
}
