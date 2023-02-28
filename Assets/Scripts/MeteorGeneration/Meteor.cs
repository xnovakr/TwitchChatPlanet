using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;

    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colourSettingsFoldout;

    public ShapeSettings shapeSettings;
    public ColourSettings colourSettings;
    public ShapeGenerator shapeGenerator = new ShapeGenerator();
    public ColourGenerator colourGenerator = new ColourGenerator();

    public bool initialized = false;
    public bool autoUpdate = true;

    public float movementSpeed = 1f;

    public ShapeCommand shapeCommand = null;
    public ColorCommand colorCommand = null;

    Material meteorMaterial;

    public Planet.FaceRenderMask faceRenderMask;
    [Range(1, 256)]
    public int resolution = 100;


    private void OnValidate()
    {
        if (initialized) GenerateMeteor();
    }
    private void Update()
    {
        float step = movementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, GameObject.Find("Planet").transform.position, step);

        Vector3 particleDirection = transform.position - GameObject.Find("Planet").transform.position;
        Quaternion rotation = Quaternion.LookRotation(particleDirection);
        transform.Find("Particle System(Clone)").rotation = rotation;
        //transform.Find("Particle System(Clone)").transform.localEulerAngles = Vector3.RotateTowards(, , -1, 10);

    }
    public void Initialize()
    {
        shapeGenerator.UpdateSettings(shapeSettings);
        colourGenerator.UpdateSettings(colourSettings);

        gameObject.AddComponent<SphereCollider>().radius = shapeSettings.planetRadius;
        gameObject.GetComponent<SphereCollider>().center = Vector3.zero;

        foreach (Transform child in transform)
        {
            if (child.GetComponent<ParticleSystem>() != null)
            {
                var particleOffset = child.GetComponent<ParticleSystem>().shape;
                particleOffset.position = new Vector3(0, 0, shapeSettings.planetRadius);
                break;
            }
        }

        int shapeFaces = 6; //faces of the cube
        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[shapeFaces];
        }
        terrainFaces = new TerrainFace[shapeFaces];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < shapeFaces; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject mesObj = new GameObject("mesh");
                mesObj.transform.parent = transform;

                mesObj.AddComponent<MeshRenderer>();
                meshFilters[i] = mesObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }
            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colourSettings.planetMaterial;

            terrainFaces[i] = new TerrainFace(shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);
            bool renderFace = faceRenderMask == Planet.FaceRenderMask.All || (int)faceRenderMask - 1 == i;
            meshFilters[i].gameObject.SetActive(renderFace);
        }
    }

    public void GenerateMeteor()
    {
        meteorMaterial = new Material(GameObject.Find("Planet").GetComponent<PlanetController>().meteorShader);
        colourSettings.planetMaterial = meteorMaterial;
        RandomizeShapeSettings();
        RandomizeColorSettings();
        shapeSettings.planetRadius = Random.Range(.1f, .4f);
        Initialize();
        GenerateMesh();
        GenerateColours();
        initialized = true;
    }

    public void GenerateMesh()
    {
        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i].gameObject.activeSelf)
            {
                terrainFaces[i].ConstructMesh();
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

    private void RandomizeShapeSettings()
    {
        shapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.centre.x = Random.Range(0f, 99999f);
        shapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.centre.y = Random.Range(0f, 99999f);
        shapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.centre.z = Random.Range(0f, 99999f);

        shapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.baseRoughness = Random.Range(0.5f, 1.5f);
        shapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.roughness = Random.Range(0.5f, 2.5f);
        shapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings.persistance = Random.Range(0.2f, 1f);
    }
    private void RandomizeColorSettings()
    {
        GradientColorKey[] colorKey;
        GradientAlphaKey[] alphaKey;

        colorKey = new GradientColorKey[2];
        alphaKey = new GradientAlphaKey[2];

        colorKey = GetRandomColorKeys(colorKey);
        alphaKey = GetRandomAlphaKeys(alphaKey);
        colourSettings.biomeColourSettings.biomes[0].gradient.SetKeys(colorKey, alphaKey);

        colorKey = GetRandomColorKeys(colorKey);
        alphaKey = GetRandomAlphaKeys(alphaKey);
        colourSettings.oceanColour.SetKeys(colorKey, alphaKey);
    }
    public GradientColorKey[] GetRandomColorKeys(GradientColorKey[] colorKey)
    {
        colorKey[0].color = GenerateRandomColor(.9f, .5f, .9f);
        colorKey[0].time = 0.0f;
        colorKey[1].color = GenerateRandomColor(.9f, .5f, .9f);
        colorKey[1].time = 1.0f;
        return colorKey;
    }
    public GradientAlphaKey[] GetRandomAlphaKeys(GradientAlphaKey[] alphaKey)
    {
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 1.0f;
        return alphaKey;
    }
    public Color GenerateRandomColor(float maxR, float maxG, float maxB)
    {//float ranges 0..1
        return new Color(
          Random.Range(0f, maxR),
          Random.Range(0f, maxG),
          Random.Range(0f, maxB)
          );
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (shapeCommand != null)
        {
            GameObject.Find("Planet").GetComponent<Planet>().ApplyShapeCommand(shapeCommand);
        }
        if (colorCommand != null)
        {
            GameObject.Find("Planet").GetComponent<Planet>().ApplyColorCommand(colorCommand);
        }
        Destroy(gameObject);
    }
}
