//
//  NativeLocation.swift
//  ARGPS Native Location
//
//  Created by Daniel M B F Manoel on 29/01/19.
//  Copyright © 2019 Daniel M B F Manoel. All rights reserved.
//

import Foundation
import CoreLocation

@objc public class NativeLocation : NSObject, CLLocationManagerDelegate {
    @objc static let shared = NativeLocation()
    
    let locationManager = CLLocationManager()
    
    var isEnabled = false;
    
    var failed = false;
    
    var currentLocation = CLLocation()
    var currentHeading = CLHeading()
    
    @objc func start(updateDistance: Double = 1) {
        locationManager.requestWhenInUseAuthorization()
        
        locationManager.desiredAccuracy = kCLLocationAccuracyBestForNavigation
        locationManager.distanceFilter = updateDistance
        locationManager.delegate = self
        locationManager.startUpdatingLocation()
        locationManager.startUpdatingHeading()
    }
    
    public func locationManager(_ manager: CLLocationManager, didChangeAuthorization status: CLAuthorizationStatus) {
        
        let authorizationStatus = CLLocationManager.authorizationStatus()
        
        if authorizationStatus == .authorizedAlways || authorizationStatus == .authorizedWhenInUse {
            isEnabled = true
        }
    }
    
    public func locationManager(_ manager: CLLocationManager, didUpdateLocations locations: [CLLocation]) {
        let latestLocation: CLLocation = locations[locations.count - 1]
        
        currentLocation = latestLocation
    }
    
    public func locationManager(_ manager: CLLocationManager, didUpdateHeading newHeading: CLHeading) {
        currentHeading = newHeading
    }
    
    @objc func getCurrentLatitude() -> Double {
        return currentLocation.coordinate.latitude
    }
    
    @objc func getCurrentLongitude() -> Double {
        return currentLocation.coordinate.longitude
    }
    
    @objc func getCurrentAltitude() -> Double {
        return currentLocation.altitude
    }
    
    @objc func getCurrentHorizontalAccuracy() -> Double {
        return currentLocation.horizontalAccuracy
    }
    
    @objc func getCurrentVerticalAccuracy() -> Double {
        return currentLocation.verticalAccuracy
    }
    
    @objc func getCurrentFloorLevel() -> Int {
        guard let level = currentLocation.floor?.level else { return -1 }
        return level
    }
    
    @objc func getCurrentTimestamp() -> Double {
        return currentLocation.timestamp.timeIntervalSince1970
    }
    
    @objc func getCurrentHeading() -> Double {
        return currentHeading.trueHeading
    }
    
    @objc func getCurrentMagneticHeading() -> Double {
        return currentHeading.magneticHeading
    }
    
    @objc func getCurrentHeadingAccuracy() -> Double {
        return currentHeading.headingAccuracy
    }
    
    @objc func getCurrentHeadingTimestamp() -> Double {
        return currentHeading.timestamp.timeIntervalSince1970
    }
    
    public func locationManager(_ manager: CLLocationManager, didFailWithError error: Error) {
        failed = true;
    }
    
    @objc func getIsEnabled() -> Bool {
        return isEnabled
    }
    
    @objc func getFailed() -> Bool {
        return failed
    }
    
    @objc func locationServicesEnabled() -> Bool {
        return CLLocationManager.locationServicesEnabled()
    }
    
    @objc func headingAvailable() -> Bool {
        return CLLocationManager.headingAvailable()
    }
}
