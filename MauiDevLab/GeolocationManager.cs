// GeolocationManager.cs

using System.Diagnostics;
using CommunityToolkit.Mvvm.Messaging;

namespace MauiDevLab;

public static class GeolocationManager
{
	static InternalListener? listener;

	public static async Task<Location?> GetLastKnownLocationAsync()
	{
		return await Geolocation.Default.GetLastKnownLocationAsync();
	}

	public static async Task StartListeningForegroundAsync()
	{
		if (listener is not null)
		{
			return;
		}

		listener = new InternalListener();
		listener.Register();

		var request = new GeolocationListeningRequest(GeolocationAccuracy.Medium);

		try
		{
			var success = await Geolocation.Default.StartListeningForegroundAsync(request);
		}
		catch (Exception ex)
		{
			Trace.TraceError($"Error starting geolocation listening: {ex}");
			listener.Unregister();
			listener = null;
		}
	}

	public static void StopListening()
	{
		if (listener is null)
		{
			return;
		}

		try
		{
			Geolocation.Default.StopListeningForeground();
		}
		catch (Exception ex)
		{
			Trace.TraceError($"Error stopping geolocation listening: {ex}");
		}
		finally
		{
			listener.Unregister();
			listener = null;
		}
	}

	class InternalListener
	{
		public void Register()
			=> Geolocation.Default.LocationChanged += OnLocationChanged;
		public void Unregister()
			=> Geolocation.Default.LocationChanged -= OnLocationChanged;
		void OnLocationChanged(object? sender, GeolocationLocationChangedEventArgs e)
		{
			Trace.WriteLine($"Location changed: {e.Location}");
			WeakReferenceMessenger.Default.Send(new LocationChangedMessage(e.Location));
		}
	}
}

public record LocationChangedMessage(Location Location)
{
}
