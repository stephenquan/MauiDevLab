// LocalizationManager.cs

using System.ComponentModel;
using System.Globalization;
using CommunityToolkit.Mvvm.Messaging;

namespace MauiDevLab;

public class LocalizationManager : INotifyPropertyChanged
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
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;
}
