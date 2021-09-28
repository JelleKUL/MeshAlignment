# MeshAlignment
Tools to help align meshes


## Table Of Contents {ignore=true}
<!-- @import "[TOC]" {cmd="toc" depthFrom=2 depthTo=6 orderedList=false} -->

<!-- code_chunk_output -->

- [Positioning](#positioning)
  - [Global Positioning](#global-positioning)
    - [Getting the GPS Coordinates](#getting-the-gps-coordinates)
    - [Converting the Coordinates for Geo-reference](#converting-the-coordinates-for-geo-reference)
  - [Local Positioning & alignment](#local-positioning-alignment)
  - [Fine Positioning](#fine-positioning)
- [Communication](#communication)
  - [Http Server](#http-server)
    - [UnityHttpListener](#unityhttplistenermainruntimecommunicationunityhttplistenercs)
    - [UnityHttpSender](#unityhttpsendermainruntimecommunicationunityhttpsendercs)

<!-- /code_chunk_output -->


## Positioning

The position is determined in 3 steps: [Global](#global-positioning), [Local](#local-positioning-alignment) and [Fine](#fine-positioning) positioning.

### Global Positioning

Global positioning places the user at a location with a targeted error radius of 20m. This enables the ability to guess the current building the user is in. And for larger buildings, take a spherical section to get a smaller test volume for the later steps.

#### Getting the GPS Coordinates

If the device has a GPS module on board, then getting the GPS coordinates is easy with Unity's build in [Location Services](https://docs.unity3d.com/ScriptReference/LocationService.html). If the device doesn't have a GPS, the data can be send from a compatible device to the target device. Check out [Communication](#communication) on how to do this.

#### Converting the Coordinates for Geo-reference

The recieved [LocationInfo](https://docs.unity3d.com/ScriptReference/LocationInfo.html) data contains latitude and altitude data. General georeferenced objects use one of the following Coordinate systems:

- Belgium
    - Lambert 72
    - Lambert 2008

Each one of these coordinate systems can be converted from and to with the [CoordinateConverter](../main/Runtime/Positioning/CoordinateConverter.cs).

The [LocationInfo](https://docs.unity3d.com/ScriptReference/LocationInfo.html) also provides an Accuracy in both directions. This information can be used to create a sphere of possible positions the device is currently in.
This sphere can be used in the next steps to calculate a local position.

### Local Positioning & alignment

### Fine Positioning

## Communication

### Http Server

Because not all devices have a GPS module, it can be difficult to get a global position.
This can be solved by sending the geo location to the device from another device, one which does have a GPS.

This is enabled with 2 scripts:

#### [UnityHttpListener](../main/Runtime/Communication/UnityHttpListener.cs)

This starts up a server @ the local IP adress, given a desired port.

#### [UnityHttpSender](../main/Runtime/Communication/UnityHttpSender.cs)

Send either Get or post requests to a certain IP adress and port.
Post request contain serialized Json data.