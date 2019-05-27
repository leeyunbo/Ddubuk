# Unity AR+GPS Location

The `AR+GPS Location` package brings the ability to position 3D objects in
real-world geographical locations via their GPS coordinates using Unity and
Augmented-Reality. It currently works Unity's `AR Foundation`, but we are working on getting it to work with the `Unity ARKit` plugin in the next releases.

This project is in it's first versions and we need a lot of feedback to 
make it as useful as possible for everyone! Bug reports and feature requests
are more than welcomed and will be implemented swiftly.

If you purchase this package you get full access to the github repository. Just
send an email to daniel.mbfm@gmail.com with the code in the ACCESS_CODE.TXT file
and your github username/email. 

## Main Features
* Place 3D Objects in geographical positions defined by their latitude, longitude and altitude.
* Place 3D Text markers on real-world points of interest (example using OpenStreetmaps is included.)
* Smooth movements on device location and heading updates.
* Move objects or place them along paths (Catmull-rom splines) on the map.
* Augmented reality floor shadows.
* Double precision vector structs, `DVector2` and `DVector3`.
* General purpose Catmull-rom curves and splines.

## Sample Scenes

* **Scenes/ARLocation Basic**: A Basic scene with one positioned object.
* **Scenes/ARLocation 3D Text**: Shows how to place 3D on points of interest on the map. You can either add them manually on the inspector, load a xml file from OpenStreetMaps/Overpass, or fetch them from the internet via a Overpass API request.
* **Scenes/ARLocation Jet Fighter** and **Scenes/ARLocation Jet Fighter Squad**: Shows a jet fighter (a jet squad in the second) flight along a predefined route on the map.
* **Scenes/ARLocation Walking Dead**: A Zombie walking around your neighborhood!
* **Scenes/ARLocation Path Line Render**: Using a line-renderer to render a ARLocationPath.
* **Scenes/ARLocation Place At Locations**: Places a prefab in a number of predefined locations.

## Limitations

* Altitude information is usually very imprecise so, currently, it's best to use heights relative to the device position.
* If the user is moving, after some distance the scene orientation and true north direction may start to deteriorate in quality. To bypass that, there is a option to reset the AR Session after the user has walked some distance from the initial position.
* Due to GPS precision, the position data can jump around a lot, making object jump round in the scene. We use movement smoothing to mitigate the effects of this.
* Movement smoothing must be used lightly on objects moving along paths. Use values around 10.0f.

## Roadmap
* `Unity ARKit` plugin support.
* *AR Hotstpots*: Regular AR experiences (e.g., using plane detection) triggered at specific locations.
* Dynamic floor height/level calculation by using nearest detected planes.
* Double precision location data by using native modules.
* Add more curve/spline types (Only Catmull-rom splines currently.)
* Improve movement smoothing (i.e., of movement due to location changes) on object moving along paths.
* Implement closed curves/paths.

## Documentation

Read the full documentation [here](https://docs.unity-ar-gps-location.com).

# Contact

If you have any questions, contact me via e-mail at <daniel.mbfm@gmail.com>, at twitter [@daniel_mbfm](https://twitter.com/daniel_mbfm), or at my 
website [danielfortes.com](https://danielfortes.com).


*Copyright © 2018 Daniel Fortes.*