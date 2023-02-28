using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;
    public enum FaceRenderMask { All, Top, Bootom, Left, Right, Front, Back};
    public FaceRenderMask faceRenderMask;

    public static Planet InstancePlanet { get; private set; }

    [Range (2,256)]
    public int resolution = 10;
    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colourSettingsFoldout;
    public bool autoUpdate = true;

    public ShapeSettings shapeSettings;
    public ColourSettings colourSettings;
    public ShapeGenerator shapeGenerator = new ShapeGenerator();
    public ColourGenerator colourGenerator = new ColourGenerator();
    [SerializeField]
    private ShapeSettings backupShapeSettings;
    private void Awake()
    {
        GeneratePlanet();
    }
    private void OnValidate()
    {
        GeneratePlanet();
    }

    public void Initialize()
    {
        shapeGenerator.UpdateSettings(shapeSettings);
        colourGenerator.UpdateSettings(colourSettings);

        if (gameObject.GetComponent<SphereCollider>() == null)
        {
            gameObject.AddComponent<SphereCollider>();
        }
        gameObject.GetComponent<SphereCollider>().radius = shapeSettings.planetRadius;

        int shapeFaces = 6; //faces of the cube
        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[shapeFaces];
        }
        terrainFaces = new TerrainFace[shapeFaces];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for(int i = 0; i < shapeFaces; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject mesObj = new GameObject("mesh");
                mesObj.transform.parent = transform;

                mesObj.AddComponent<MeshRenderer>();
                meshFilters[i] = mesObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();

                //mesObj.AddComponent<MeshCollider>().convex = true;
            }
            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colourSettings.planetMaterial;

            terrainFaces[i] = new TerrainFace(shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);
            bool renderFace = faceRenderMask == FaceRenderMask.All || (int)faceRenderMask - 1 == i;
            meshFilters[i].gameObject.SetActive(renderFace);
        }
    }

    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColours();
    }

    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }

    public void OnColourSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateColours();
        }
    }

    public void GenerateMesh()
    {
        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i].gameObject.activeSelf)
            {
                terrainFaces[i].ConstructMesh();
                //meshFilters[i].gameObject.GetComponent<MeshCollider>().sharedMesh = terrainFaces[i].GetMesh();//meshFilters[i].sharedMesh;
            }
        }

        colourGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
    }

    public void GenerateColours()
    {
        colourGenerator.UpdateColours();
        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i].gameObject.activeSelf)
            {
                terrainFaces[i].UpdateUVs(colourGenerator);
            }
        }
    }

    private bool ValueCheck(float value)
    {
        return value == 0 ? false : true;
    }
    public void ApplyShapeCommand(ShapeCommand shapeCommand)
    {
        if (ValueCheck(shapeCommand.resolution)) resolution += shapeCommand.resolution;

        if (ValueCheck(shapeCommand.planetRadius)) shapeSettings.planetRadius += shapeCommand.planetRadius;

        if(ValueCheck(shapeCommand.strenght)) shapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.strenght += shapeCommand.strenght;
        if (ValueCheck(shapeCommand.baseRoughness)) shapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.baseRoughness += shapeCommand.baseRoughness;
        if (ValueCheck(shapeCommand.roughness)) shapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.roughness += shapeCommand.roughness;
        if (ValueCheck(shapeCommand.persistance)) shapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.persistance += shapeCommand.persistance;

        if (ValueCheck(shapeCommand.noiseOffsetX)) shapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.centre.x += shapeCommand.noiseOffsetX;
        if (ValueCheck(shapeCommand.noiseOffsetY)) shapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.centre.y += shapeCommand.noiseOffsetY;
        if (ValueCheck(shapeCommand.noiseOffsetZ)) shapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.centre.z += shapeCommand.noiseOffsetZ;
        
        if (ValueCheck(shapeCommand.minValue)) shapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.minValue += shapeCommand.minValue;//TODO simple/ridgid
        OnShapeSettingsUpdated();
    }
    public void ApplyColorCommand(ColorCommand colorCommand)
    {
        Color color = new Color(0, 0, 0);
        if (ValueCheck(colorCommand.red)) color.r = 0.01f;
        if (ValueCheck(colorCommand.green)) color.g = 0.01f;
        if (ValueCheck(colorCommand.blue)) color.b = 0.01f;

        if (ValueCheck(colorCommand.alfa)) color.a = 0.01f;

        if (color != new Color(0, 0, 0))
        {
            Gradient colorGradient = colourSettings.biomeColourSettings.biomes[0].gradient;
            Color newColor = CombineColors(colorGradient.colorKeys[0].color, color);
            colourSettings.biomeColourSettings.biomes[0].gradient.colorKeys[0].color = newColor;
            newColor = CombineColors(colorGradient.colorKeys[1].color, color);
            colourSettings.biomeColourSettings.biomes[0].gradient.colorKeys[1].color = newColor;
        }
        OnColourSettingsUpdated();
    }
    private Color CombineColors(Color color1, Color color2)
    {
        Color returnColor = new Color();
        returnColor.r = color1.r + color2.r;
        returnColor.g = color1.g + color2.g;
        returnColor.b = color1.b + color2.b;
        returnColor.a = color1.a + color2.a;
        return returnColor;
    }
    public void ResetNoiseSettings()
    {
        resolution = 140;
        shapeSettings.planetRadius = backupShapeSettings.planetRadius;
        shapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.strenght = backupShapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.strenght;
        shapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.baseRoughness = backupShapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.baseRoughness;
        shapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.roughness = backupShapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.roughness;
        shapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.persistance = backupShapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.persistance;

        shapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.centre.x = backupShapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.centre.x;
        shapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.centre.y = backupShapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.centre.y;
        shapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.centre.z = backupShapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.centre.z;

        shapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.minValue = backupShapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.minValue;//TODO simple/ridgid
        OnShapeSettingsUpdated();
    }
    public void ChangePlanetSize(float value)
    {
        shapeSettings.planetRadius += value;
        //var camPos = Camera.main.gameObject.transform.position;
        //camPos.z += -(value * 10);
        //Camera.main.transform.position = camPos;
        OnShapeSettingsUpdated();
    }
    public void ChangePlanetResolution(int value)
    {
        resolution += value;
        OnShapeSettingsUpdated();
    }
}
