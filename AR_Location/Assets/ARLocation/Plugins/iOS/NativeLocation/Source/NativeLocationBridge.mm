//
//  NativeLocationBridge.m
//
//  Created by Daniel M B F Manoel on 29/01/19.
//  Copyright © 2019 Daniel M B F Manoel. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreLocation/CoreLocation.h>
#import "NativeLocation-Swift.h"

#pragma mark - C interface

extern "C" {
    void _nl_start(double updateDistance) {
        [[NativeLocation shared] startWithUpdateDistance:(double)updateDistance];
    }
    
    double _nl_get_current_latitude() {
        return [[NativeLocation shared] getCurrentLatitude];
    }
    
    double _nl_get_current_longitude() {
        return [[NativeLocation shared] getCurrentLongitude];
    }
    
    double _nl_get_current_altitude() {
        return [[NativeLocation shared] getCurrentAltitude];
    }
    
    double _nl_get_current_horizontal_accuracy() {
        return [[NativeLocation shared] getCurrentHorizontalAccuracy];
    }
    
    double _nl_get_current_vertical_accuracy() {
        return [[NativeLocation shared] getCurrentVerticalAccuracy];
    }
    
    int _nl_get_current_floor_level() {
        return [[NativeLocation shared] getCurrentFloorLevel];
    }
    
    int _nl_get_current_timestamp() {
        return [[NativeLocation shared] getCurrentTimestamp];
    }
    
    double _nl_get_current_heading() {
        return [[NativeLocation shared] getCurrentHeading];
    }
    
    double _nl_get_current_magnetic_heading() {
        return [[NativeLocation shared] getCurrentMagneticHeading];
    }
    
    double _nl_get_current_heading_accuracy() {
        return [[NativeLocation shared] getCurrentHeadingAccuracy];
    }
    
    double _nl_get_current_heading_timestamp() {
        return [[NativeLocation shared] getCurrentHeadingTimestamp];
    }
    
    bool _nl_location_services_enabled() {
        return [[NativeLocation shared] locationServicesEnabled];
    }
    
    bool _nl_heading_available() {
        return [[NativeLocation shared] headingAvailable];
    }
    
    bool _nl_get_is_enabled() {
        return [[NativeLocation shared] getIsEnabled];
    }
    
    bool _nl_get_failed() {
        return [[NativeLocation shared] getFailed];
    }
}
