using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class CommandFactory
{
    private static char SEPARATOR = '|';
    public static void SelectCommand(string msg)
    {
        string[] splitMsg = msg.Split(SEPARATOR);
        CommandingClasses.Commands commandEnum = CommandingClasses.GetCommandEnum(splitMsg[0]);
        GameObject meteor;

        switch (commandEnum)
        {
            case CommandingClasses.Commands.Meteor:
                GameObject.Find("Planet").GetComponent<PlanetController>().SpawnMeteor();
                PipeConnection_Server.Instasnce.SendCommand("Spwaning empty Meteor!");
                Debug.Log("Spwaning empty Meteor!");
                return;
            case CommandingClasses.Commands.Noise:
                meteor = GameObject.Find("Planet").GetComponent<PlanetController>().SpawnMeteor();
                //GameObject.Find("Planet").GetComponent<Planet>().ApplyShapeCommand(splitMsg[1]);
                meteor.GetComponent<Meteor>().shapeCommand = JsonUtility.FromJson<ShapeCommand>(splitMsg[1]);
                PipeConnection_Server.Instasnce.SendCommand("Spwaning noise Meteor!");
                Debug.Log("Spwaning noise Meteor!");
                return;
            case CommandingClasses.Commands.Color:
                meteor = GameObject.Find("Planet").GetComponent<PlanetController>().SpawnMeteor();
                meteor.GetComponent<Meteor>().colorCommand = JsonUtility.FromJson<ColorCommand>(splitMsg[1]);
                PipeConnection_Server.Instasnce.SendCommand("Spwaning color Meteor!");
                Debug.Log("Spwaning color Meteor!");
                return;
            case CommandingClasses.Commands.Resolution:
                GameObject.Find("Planet").GetComponent<Planet>().ChangePlanetResolution(int.Parse(splitMsg[1]));
                PipeConnection_Server.Instasnce.SendCommand("Changing planet reoslution!");
                Debug.Log("Changing planet resolution!");
                break;
            case CommandingClasses.Commands.Size:
                GameObject.Find("Planet").GetComponent<Planet>().ChangePlanetSize((float)decimal.Parse(splitMsg[1], CultureInfo.InvariantCulture.NumberFormat));
                PipeConnection_Server.Instasnce.SendCommand("changing planet Size!");
                Debug.Log("Chanigng planetSize!");
                break;
            case CommandingClasses.Commands.CameraMovement:
                MoveCamera(splitMsg[1]);
                PipeConnection_Server.Instasnce.SendCommand("Moving main camera!");
                Debug.Log("Moving camera!");
                return;
            case CommandingClasses.Commands.PlanetReset:
                GameObject.Find("Planet").GetComponent<Planet>().ResetNoiseSettings();
                PipeConnection_Server.Instasnce.SendCommand("Reseting shape settings!");
                Debug.Log("Reseting shape settings!");
                return;
            case CommandingClasses.Commands.NULL:
                //Debug.Log(splitMsg[splitMsg.Length - 1]);
                PipeConnection_Server.Instasnce.SendCommand("Invalid Command!\n" + splitMsg[splitMsg.Length - 1]);
                return;
            case CommandingClasses.Commands.Quit:
                PipeConnection_Server.Instasnce.SendCommand("Closing planet Application!");
                Application.Quit();
                return;
            default:
                return;
        }
    }
    private static void MoveCamera(string command)
    {
        CameraCommand cameraCommand = JsonUtility.FromJson<CameraCommand>(command);
        var camPos = Camera.main.gameObject.transform.position;
        if (cameraCommand.z != 0) camPos.z += cameraCommand.z;
        Camera.main.gameObject.transform.position = camPos;
    }
}
