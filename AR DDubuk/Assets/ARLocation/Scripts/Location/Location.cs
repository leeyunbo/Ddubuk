using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Represents a geographical location.
/// </summary>
[System.Serializable]
public class Location
{
    /// <summary>
    /// The latitude, in degrees.
    /// </summary>
    [Tooltip("The latitude, in degrees.")]
    public double latitude;

    /// <summary>
    /// The longitude, in degrees.
    /// </summary>
    [Tooltip("The longitude, in degrees.")]
    public double longitude;

    /// <summary>
    /// The altitude, in meters.
    /// </summary>
    [Tooltip("The altitude, in meters.")]
    public double altitude;

    /// <summary>
    /// If true, the altitude will be ignored when placing an object, and the 
    /// object's will be placed at the same elevation as the device.
    /// </summary>
    [Tooltip("If true, the altitude will be ignored when placing an object, and the object's will be placed at the same elevation as the device.")]
    public bool ignoreAltitude = false;

    /// <summary>
    /// An optional label to the location
    /// </summary>
    [Tooltip("An optional label for the location.")]
    public string label = "";

    /// <summary>
    /// Gets the horizontal vector.
    /// </summary>
    /// <value>The horizontal vector.</value>
    public DVector2 horizontalVector
    {
        get
        {
            return new DVector2(latitude, longitude);
        }
    }

    public Location(double latitude = 0.0, double longitude = 0.0, double altitude = 0.0)
    {
        this.latitude = latitude;
        this.longitude = longitude;
        this.altitude = altitude;
    }

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>The clone.</returns>
    public Location Clone()
    {
        return new Location(latitude, longitude, altitude);
    }

    override public string ToString()
    {
        return "(" + latitude + ", " + longitude + ", " + altitude + ")";
    }

    public DVector3 ToDVector3() {
        return new DVector3(longitude, altitude, latitude);
    }

    public Vector3 ToVector3() {
        return ToDVector3().toVector3();
    }

    /// <summary>
    /// Calculates the horizontal distance according to the current function
    /// set in the configuration.
    /// </summary>
    /// <returns>The distance, in meters.</returns>
    /// <param name="l1">L1.</param>
    /// <param name="l2">L2.</param>
    public static double HorizontalDistance(Location l1, Location l2)
    {
        var type = ARLocation.config.DistanceFunction;

        switch (type)
        {
            case ARLocationConfig.ARLocationDistanceFunc.Haversine:
                return HaversineDistance(l1, l2);
            case ARLocationConfig.ARLocationDistanceFunc.PlaneSpherical:
                return PlaneSphericalDistance(l1, l2);
            case ARLocationConfig.ARLocationDistanceFunc.PlaneEllipsoidalFCC:
                return PlaneEllipsoidalFCCDistance(l1, l2);
            default:
                return HaversineDistance(l1, l2);
        }
    }

    /// <summary>
    /// Horizontal distance using spherical projection on a plane.
    /// https://en.wikipedia.org/wiki/Geographical_distance
    /// </summary>
    /// <returns>The distance, in meters.</returns>
    /// <param name="l1"></param>
    /// <param name="l2"></param>
    /// <returns></returns>
    public static double PlaneSphericalDistance(Location l1, Location l2)
    {
        var R = ARLocation.config.EarthRadiusInKM;
        var rad = Math.PI / 180;
        var dLat = (l2.latitude - l1.latitude) * rad;
        var dLon = (l2.longitude - l1.longitude) * rad;
        var lat1 = l1.latitude * rad;
        var lat2 = l2.latitude * rad;
        var mLat = (lat1 + lat2) / 2.0;
        var mLatC = Math.Cos(mLat);

        var a = dLat * dLat;    
        var b = mLatC * mLatC * dLon * dLon;

        return R * Math.Sqrt(a + b) * 1000.0;
    }

    /// <summary>
    /// Horizontal distance using ellipsoidal projection on a plane.
    /// https://en.wikipedia.org/wiki/Geographical_distance
    /// </summary>
    /// <returns>The distance, in meters.</returns>
    /// <param name="l1"></param>
    /// <param name="l2"></param>
    /// <returns></returns>
    public static double PlaneEllipsoidalFCCDistance(Location l1, Location l2)
    {
        var R = ARLocation.config.EarthRadiusInKM;
        var rad = Math.PI / 180;
        var dLat = (l2.latitude - l1.latitude) * rad;
        var dLon = (l2.longitude - l1.longitude) * rad;
        var lat1 = l1.latitude * rad;
        var lat2 = l2.latitude * rad;
        var mLat = (lat1 + lat2) / 2.0;
        var mLatC = Math.Cos(mLat);

        var K1 = 111.13209 - 0.56605 * Math.Cos(2 * mLat) + 0.00120 * Math.Cos(4 * mLat);
        var K2 = 111.41513 * Math.Cos(mLat) - 0.09455 * Math.Cos(3 * mLat) + 0.00012 * Math.Cos(5 * mLat);

        var a = K1 * (l2.latitude - l1.latitude);
        var b = K2 * (l2.longitude - l1.longitude);

        return 1000.0 * Math.Sqrt(a * a + b * b);
    }

    /// <summary>
    /// Horizontal distance, using the Haversine formula.
    /// https://stackoverflow.com/questions/41621957/a-more-efficient-haversine-function
    /// </summary>
    /// <returns>The distance, in meters.</returns>
    /// <param name="l1">L1.</param>
    /// <param name="l2">L2.</param>
    public static double HaversineDistance(Location l1, Location l2)
    {
        var R = ARLocation.config.EarthRadiusInKM;
        var rad = Math.PI / 180;
        var dLat = (l2.latitude - l1.latitude) * rad;
        var dLon = (l2.longitude - l1.longitude) * rad;
        var lat1 = l1.latitude * rad;
        var lat2 = l2.latitude * rad;

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);

        return R * 2 * Math.Asin(Math.Sqrt(a)) * 1000;
    }

    
    /// <summary>
    /// Calculates the full distance between locations, taking altitude into account.
    /// </summary>
    /// <returns>The with altitude.</returns>
    /// <param name="l1">L1.</param>
    /// <param name="l2">L2.</param>
    public static double DistanceWithAltitude(Location l1, Location l2)
    {
        var d = HorizontalDistance(l1, l2);
        var h = Math.Abs(l1.altitude - l2.altitude);

        return Math.Sqrt(d * d + h * h);
    }

    /// <summary>
    /// Calculates the horizontal vector pointing from l1 to l2, in meters.
    /// </summary>
    /// <returns>The vector from to.</returns>
    /// <param name="l1">L1.</param>
    /// <param name="l2">L2.</param>
    public static DVector2 HorizontalVectorFromTo(Location l1, Location l2)
    {
        var d = HorizontalDistance(l1, l2);

        var direction = (l2.horizontalVector - l1.horizontalVector).normalized;

        return direction * d;
    }

    /// <summary>
    /// Calculates the vector from l1 to l2, in meters, taking altitude into accound.
    /// </summary>
    /// <returns>The from to.</returns>
    /// <param name="l1">L1.</param>
    /// <param name="l2">L2.</param>
    /// <param name="ignoreHeight">If true, y = 0 in the output vector.</param>
    public static DVector3 VectorFromTo(Location l1, Location l2, bool ignoreHeight = false)
    {
        var horizontal = HorizontalVectorFromTo(l1, l2);
        var height = l2.altitude - l1.altitude;

        return new DVector3(horizontal.y, ignoreHeight ? 0 : height, horizontal.x);
    }

    /// <summary>
    /// Gets the game object position for location.
    /// </summary>
    /// <param name="userPosition"></param>
    /// <param name="userLocation"></param>
    /// <param name="objectLocation"></param>
    /// <param name="heightIsRelative"></param>
    /// <returns></returns>
    public static Vector3 GetGameObjectPositionForLocation(Vector3 userPosition, Location userLocation, Location objectLocation, bool heightIsRelative)
    {
        var dpos = Location.VectorFromTo(userLocation, objectLocation, objectLocation.ignoreAltitude || heightIsRelative);
        return userPosition + dpos.toVector3() + new Vector3(0, (heightIsRelative && !objectLocation.ignoreAltitude) ? (float)objectLocation.altitude : 0, 0);
    }

    /// <summary>
    /// Gets the game object position for location.
    /// </summary>
    /// <returns>The game object position for location.</returns>
    /// <param name="user">User.</param>
    /// <param name="userLocation">User location.</param>
    /// <param name="objectLocation">Object location.</param>
    /// <param name="heightIsRelative">If set to <c>true</c> height is relative.</param>
    /// 
    public static Vector3 GetGameObjectPositionForLocation(Transform user, Location userLocation, Location objectLocation, bool heightIsRelative)
    {
        var dpos = Location.VectorFromTo(userLocation, objectLocation, objectLocation.ignoreAltitude || heightIsRelative);
        return user.position + dpos.toVector3() + new Vector3(0, (heightIsRelative && !objectLocation.ignoreAltitude) ? (float)objectLocation.altitude : 0, 0);
    }

    /// <summary>
    /// Places the game object at location.
    /// </summary>
    /// <param name="transform">The GameObject's transform.</param>
    /// <param name="user">The user's point of view Transform, e.g., camera.</param>
    /// <param name="userLocation">User Location.</param>
    /// <param name="objectLocation">Object Location.</param>
    public static void PlaceGameObjectAtLocation(Transform transform, Transform user, Location userLocation, Location objectLocation, bool heightIsRelative)
    {
        transform.localPosition = GetGameObjectPositionForLocation(user, userLocation, objectLocation, heightIsRelative);
    }

    static public bool Equal(Location a, Location b, double eps = 0.0000001)
    {
        return (Math.Abs(a.latitude - b.latitude) <= eps) &&
            (Math.Abs(a.longitude - b.longitude) <= eps) &&
            (Math.Abs(a.altitude - b.altitude) <= eps);
    }
}
