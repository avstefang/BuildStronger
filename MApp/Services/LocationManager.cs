using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui.Devices.Sensors;

namespace MApp.Services;

public class LocationManager
{
    public async Task<Location?> GetCurrentLocationAsync()
    {
        try
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Medium);
            var location = await Geolocation.GetLocationAsync(request);
            return location;
        }
        catch
        {
            return null;
        }
    }
}