using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HeadingReading {
    public double heading;
    public double magneticHeading;
    public double accuracy;
    public long timestamp;
    public bool isMagneticHeadingAvailable;

    HeadingReading(
        double heading = 0.0,
        double accuracy = 0,
        long timestamp = 0,
        bool isMagneticHeadingAvailable = false,
        double magneticHeading = 0.0
        )
    {
        this.heading = heading;
        this.accuracy = accuracy;
        this.isMagneticHeadingAvailable = isMagneticHeadingAvailable;
        this.magneticHeading = magneticHeading;
        this.timestamp = timestamp;
    }

    public override string ToString()
    {
        return 
            "HeadingReading {\n" +
            "  heading = " + heading + "\n" +
            "  magneticHeading = " + magneticHeading + "\n" +
            "  accuracy = " + accuracy + "\n" +
            "  timestamp = " + timestamp + "\n" +
            "  isMagneticHeadingAvailable = " + isMagneticHeadingAvailable + "\n" +
            "}";
    }
}
