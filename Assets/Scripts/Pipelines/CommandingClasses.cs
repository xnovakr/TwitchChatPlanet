using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandingClasses
{
    public enum Commands { NULL, Meteor, Noise, Color, Resolution, Size, PlanetReset, CameraMovement, Quit }
    public Commands commands;

    public static Commands GetCommandEnum(string command)
    {
        switch (command)
        {
            case "Meteor":
                return Commands.Meteor;
            case "Noise":
                return Commands.Noise;
            case "Color":
                return Commands.Color;
            case "PlanetReset":
                return Commands.PlanetReset;
            case "CameraMovement":
                return Commands.CameraMovement;
            case "Quit":
                return Commands.Quit;
            case "Resolution":
                return Commands.Resolution;
            case "Size":
                return Commands.Size;
            default:
                return Commands.NULL;
        }
    }
}

public class ShapeCommand
{
    public int resolution = 0; // planet resolution
    public int numberOfLayers = 0; // planetDetail

    public float planetRadius = 0; // planet size

    public float strenght = 0; // heigh of mountines
    public float baseRoughness = 0; // base of mountines
    public float roughness = 0; // number of mountines
    public float persistance = 0; // peaks of mountines

    public float noiseOffsetX = 0; // mountiness offset
    public float noiseOffsetY = 0;
    public float noiseOffsetZ = 0;

    public float minValue = 0; // water level
    public ShapeCommand() { }
    public ShapeCommand(float baseValue)
    {
        resolution = (int)baseValue; numberOfLayers = (int)baseValue; planetRadius = baseValue; strenght = baseValue; baseRoughness = baseValue;
        roughness = baseValue; persistance = baseValue; noiseOffsetX = baseValue; noiseOffsetY = baseValue; noiseOffsetZ = baseValue; minValue = baseValue;
    }
    public void SetValue(float baseValue)
    {
        resolution = (int)baseValue; numberOfLayers = (int)baseValue; planetRadius = baseValue; strenght = baseValue; baseRoughness = baseValue;
        roughness = baseValue; persistance = baseValue; noiseOffsetX = baseValue; noiseOffsetY = baseValue; noiseOffsetZ = baseValue; minValue = baseValue;
    }
}
public class ColorCommand
{
    public float red = 0;
    public float green = 0;
    public float blue = 0;

    public float alfa = 0;

    public ColorCommand() { }
    public ColorCommand(float red, float green, float blue, float alfa = 0)
    {
        this.red = red;
        this.green = green;
        this.blue = blue;

        this.alfa = alfa;
    }
    public void AddRed()
    {
        red = 1;
    }
    public void AddGreen()
    {
        green = 1;
    }
    public void AddBlue()
    {
        blue = 1;
    }
}
public class CameraCommand
{
    public float x = 0;
    public float y = 0;
    public float z = 0;

    public float fov = 0;

    public float rotationX = 0;
    public float rotationY = 0;
    public float rotationZ = 0;

    public CameraCommand() { }
    public CameraCommand(Vector3 position, float fov, Vector3 rotation)
    {
        x = position.x;
        y = position.y;
        z = position.z;

        this.fov = fov;

        rotationX = rotation.x;
        rotationY = rotation.y;
        rotationZ = rotation.z;
    }
}