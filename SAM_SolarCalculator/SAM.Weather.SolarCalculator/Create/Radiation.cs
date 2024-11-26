﻿using Innovative.Geometry;
using Innovative.SolarCalculator;
using SAM.Core;
using SAM.Geometry.SolarCalculator;
using SAM.Geometry.Spatial;
using System;
using System.Collections.Generic;

namespace SAM.Weather.SolarCalculator
{
    public static partial class Create
    {
        public static Radiation Radiation(WeatherData weatherData, DateTime dateTime, Plane plane, double skyViewFactor = 1, double groundViewFactor = 1, double albedo = 0.2, bool includeNight = false)
        {
            if (weatherData == null || plane == null)
            {
                return null;
            }

            Location location = weatherData.Location;

            Angle angle_Latitude = new Angle(location.Latitude);
            Angle angle_Longitude = new Angle(location.Longitude);

            int timeZoneOffset = 0;
            if (location.TryGetValue(LocationParameter.TimeZone, out string timeZoneString))
            {
                UTC uTC = Core.Query.UTC(timeZoneString);
                if (uTC != UTC.Undefined)
                {
                    timeZoneOffset = System.Convert.ToInt32(Core.Query.Double(uTC));
                }
            }

            SolarTimes solarTimes = new SolarTimes(dateTime, timeZoneOffset, angle_Latitude, angle_Longitude);
            if (!includeNight && (dateTime < solarTimes.Sunrise || dateTime > solarTimes.Sunset))
            {
                return null;
            }

            DateTime dateTime_Temp = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);

            Dictionary<DateTime, double> dictionary_GlobalSolarRadiation = weatherData.Values(WeatherDataType.GlobalSolarRadiation);
            if (dictionary_GlobalSolarRadiation == null || !dictionary_GlobalSolarRadiation.TryGetValue(dateTime_Temp, out double globalSolarRadiation))
            {
                return null;
            }

            Dictionary<DateTime, double> dictionary_DiffuseSolarRadiation = weatherData.Values(WeatherDataType.DiffuseSolarRadiation);
            if (dictionary_DiffuseSolarRadiation == null || !dictionary_DiffuseSolarRadiation.TryGetValue(dateTime_Temp, out double diffuseSolarRadiation))
            {
                return null;
            }


            Dictionary<DateTime, double> dictionary_DirectSolarRadiation = weatherData.Values(WeatherDataType.DirectSolarRadiation);
            if (dictionary_DirectSolarRadiation == null || !dictionary_DirectSolarRadiation.TryGetValue(dateTime_Temp, out double directSolarRadiation))
            {
                dictionary_DirectSolarRadiation = new Dictionary<DateTime, double>();
                dictionary_DirectSolarRadiation[dateTime_Temp] = globalSolarRadiation - diffuseSolarRadiation;
            }

            if (dictionary_DirectSolarRadiation == null || !dictionary_DirectSolarRadiation.TryGetValue(dateTime_Temp, out directSolarRadiation))
            {
                return null;
            }

            double tilt = Geometry.Spatial.Query.Tilt(plane);
            double surfaceAzimuth = Geometry.Spatial.Query.Azimuth(plane, Vector3D.WorldY); 
           
            return Geometry.SolarCalculator.Create.Radiation(solarTimes, tilt, surfaceAzimuth, directSolarRadiation, diffuseSolarRadiation, globalSolarRadiation, skyViewFactor, groundViewFactor, albedo);
        }
    }
}
