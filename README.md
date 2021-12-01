# MeshAlignment
Tools to help align meshes

```cs
 namespace JelleKUL.MeshAlignment
```

<!-- @import "[TOC]" {cmd="toc" depthFrom=2 depthTo=6 orderedList=false} -->

<!-- code_chunk_output -->

- [Installation](#installation)
- [Positioning](#positioning)
  - [Global Positioning](#global-positioning)
    - [Getting the GPS Coordinates](#getting-the-gps-coordinates)
    - [Converting the Coordinates for Geo-reference](#converting-the-coordinates-for-geo-reference)
  - [Local Positioning & alignment](#local-positioning-alignment)
    - [Workflow](#workflow)
      - [Device Inputs](#device-inputs)
      - [Reference Data](#reference-data)
      - [Results](#results)
  - [Fine Positioning](#fine-positioning)
- [Communication](#communication)
  - [Http Server](#http-server)
    - [UnityHttpListener](#unityhttplistenermainruntimecommunicationunityhttplistenercs)
    - [UnityHttpSender](#unityhttpsendermainruntimecommunicationunityhttpsendercs)
  - [Geo location](#geo-location)
- [Licensing](#licensing)

<!-- /code_chunk_output -->

## Installation

This can be imported as a UnityPackage in any existing Unity project through the [Package manager](https://docs.unity3d.com/Manual/Packages.html) with the Git url.

## Positioning

The position is determined in 3 steps: [Global](#global-positioning), [Local](#local-positioning-alignment) and [Fine](#fine-positioning) positioning.

### Global Positioning

Global positioning places the user at a location with a targeted error radius of *20m*.
This enables the ability to guess the current building the user is in. And for larger buildings, take a spherical section to get a smaller test volume for the later steps.

#### Getting the GPS Coordinates

If the device has a GPS module on board, then getting the GPS coordinates is easy with Unity's build in [Location Services](https://docs.unity3d.com/ScriptReference/LocationService.html).
If the device doesn't have a GPS, the data can be send from a compatible device to the target device. Check out [Communication](#communication) on how to do this.

The position is send over the internet in serialized Json
```cs
public class PositionInfo
    {
        public Vector3 position = Vector3.zero;
        public float errorRadius = 0f;
        public int coordinateSystem = 0;

    }
```

#### Converting the Coordinates for Geo-reference

The received [LocationInfo](https://docs.unity3d.com/ScriptReference/LocationInfo.html) data contains latitude, longitude and altitude data. General geo-referenced objects use one of the following Coordinate systems:

- Belgium
    - Lambert 72
    - Lambert 2008
    - WGS 84

Each one of these coordinate systems can be converted from and to with the [CoordinateConverter](../main/Runtime/Positioning/CoordinateConverter.cs).

The [LocationInfo](https://docs.unity3d.com/ScriptReference/LocationInfo.html) also provides an Accuracy in both directions. This information can be used to create a sphere of possible positions the device is currently in.
This sphere can be used in the next steps to calculate a local position.

### Local Positioning & alignment

After we have a rough estimate of the global position, the next step is to get a more precise location with an error margin of about *20cm*, in other words, get the correct room of the user. For this we can use features that the XR device gets from its environment, like the image data or meshes.
The Hololens has a magnetometer to determine absolute orientation. this can be used for the initial guess. (this sensor is only available in research mode)

#### Workflow

##### Device Inputs

- Geo Location ([See Global Positioning](#global-positioning))
- Partially scanned Mesh
- Location Photos 

##### Reference Data

- 3D Data
  - Point clouds
  - Meshes
- 2D Data
  - Pictures
  - Panoramas


##### Results

Different guesses where the device might be positioned in the world. Using minimal detectable bias and voting to determine the best location.


### Fine Positioning

> **todo**

## Communication

### Http Server

Because not all devices have a GPS module, it can be difficult to get a global position.
This can be solved by sending the geo location to the device from another device, one which does have a GPS.

This is enabled with 2 scripts:

#### [Httpserver](../main/Runtime/Communication/Httpserver.cs)

This starts up a server @ the local IP address, given a desired port.

#### [HttpClient](../main/Runtime/Communication/HttpClient.cs)

Send either Get or post requests to a certain IP address and port.
Post requests can contain serialized Json data.

#### [Location Sender](../main/Runtime/Samples/LocationSender.cs)

An example implementation of sending the global position to a webserver for processing

#### [File Sender](../main/Runtime/Samples/FileSender.cs)

An example implementation of sending a folder and all it's files to a server for processing

### Geo location

The geo-location is determined on a mobile device using Unity's [Location services](https://docs.unity3d.com/ScriptReference/LocationService.html), which returns a [LocationInfo](https://docs.unity3d.com/ScriptReference/LocationInfo.html).

For web apps you can use [GeoLocation API](https://www.w3schools.com/html/html5_geolocation.asp)

> For future version, the geolocation could be send directly to a cloud server. Since the bulk of the calculations are done on a seperate server, which has access to the large library of reference data

## Licensing

The code in this project is licensed under MIT license.
