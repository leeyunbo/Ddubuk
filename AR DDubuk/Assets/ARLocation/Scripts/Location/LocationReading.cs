using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LocationReading {
    public double latitude;
    public double longitude;
    public double altitude;
    public double accuracy;
    public int floor;

    /// <summary>
    /// Epoch time in ms
    /// </summary>
    public long timestamp;

    LocationReading(
        double latitude = 0.0, 
        double longitude = 0.0, 
        double altitude = 0.0, 
        float accuracy = 0.0f,
        long timestamp =-1,
        int floor = -1
    )
    {
        this.latitude = latitude;
        this.longitude = longitude;
        this.altitude = altitude;
        this.accuracy = accuracy;
        this.timestamp = timestamp;
        this.floor = floor;
    }

    public Location ToLocation()
    {
        return new Location(latitude, longitude, altitude);
    }

    public static double HorizontalDistance(LocationReading a, LocationReading b)
    {
        return Location.HorizontalDistance(a.ToLocation(), b.ToLocation());
    }

    public override string ToString()
    {
        return
            "LocationReading { \n" +
            "  latitude = " + latitude + "\n" +
            "  longitude = " + longitude + "\n" +
            "  altitude = " + altitude + "\n" +
            "  accuracy = " + accuracy + "\n" +
            "  floor = " + floor + "\n" +
            "  timestamp = " + timestamp + "\n" +
            "}";
    }
}
