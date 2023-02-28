using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetController : MonoBehaviour
{
    public ShapeSettings meteorShapeSettings;
    public ColourSettings meteorColorSettings;
    public Shader meteorShader;
    public GameObject trailParticle;

    public bool rotationToggle = true;
    public float rotationSpeed = 10f;

    void Update()
    {
        if (rotationToggle)
        {
            Vector3 rotation = gameObject.transform.eulerAngles;
            rotation.y += rotationSpeed * Time.deltaTime;
            gameObject.transform.eulerAngles = rotation;
        }
    }

    public GameObject SpawnMeteor()
    {
        GameObject meteor = new GameObject("Meteor");
        Vector3 spawningPosition = Vector3.zero + SelectMeteorSideOffset(GetRandomPlanetSide());
        InitializeMeteor(meteor, spawningPosition);
        return meteor;
    }
    private void InitializeMeteor(GameObject meteor, Vector3 spawningPos)
    {
        meteor.AddComponent<Rigidbody>().useGravity = false;
        GameObject.Instantiate(trailParticle, meteor.transform);
        meteor.transform.Find("Particle System(Clone)").transform.position = Vector3.zero;
        meteor.AddComponent<Meteor>().colourSettings = meteorColorSettings;
        meteor.GetComponent<Meteor>().shapeSettings = meteorShapeSettings;
        meteor.GetComponent<Meteor>().GenerateMeteor();
        meteor.transform.position = spawningPos*2;
    }
    private Vector3 SelectMeteorSideOffset(Planet.FaceRenderMask side)
    {
        float planetRadius = gameObject.GetComponent<Planet>().shapeSettings.planetRadius + meteorShapeSettings.planetRadius;
        float randRange = planetRadius + 2;
        float randOne = Random.Range(-randRange, randRange);
        float randTwo = Random.Range(-randRange, randRange);
        float randThree = Random.Range(0, randRange);
        switch (side)
        {
            case Planet.FaceRenderMask.All:
                return new Vector3(planetRadius + randThree, planetRadius + randThree, planetRadius + randThree);
            case Planet.FaceRenderMask.Front:
                return new Vector3(randOne, randTwo, planetRadius + randThree);
            case Planet.FaceRenderMask.Back:
                return new Vector3(randOne, randTwo, -planetRadius - randThree);
            case Planet.FaceRenderMask.Left:
                return new Vector3(-planetRadius - randThree, randOne, randTwo);
            case Planet.FaceRenderMask.Right:
                return new Vector3(planetRadius + randThree, randOne, randTwo);
            case Planet.FaceRenderMask.Top:
                return new Vector3(randOne, planetRadius + randThree, randTwo);
            case Planet.FaceRenderMask.Bootom:
                return new Vector3(randOne, -planetRadius - randThree, randTwo);
            default:
                return Vector3.zero;
        }
    }
    public Planet.FaceRenderMask GetRandomPlanetSide()
    {
        int rand = Random.Range(1, 7);
        switch (rand)
        {
            case 1:
                return Planet.FaceRenderMask.Front;
            case 2:
                return Planet.FaceRenderMask.Back;
            case 3:
                return Planet.FaceRenderMask.Left;
            case 4:
                return Planet.FaceRenderMask.Right;
            case 5:
                return Planet.FaceRenderMask.Top;
            case 6:
                return Planet.FaceRenderMask.Bootom;
            default:
                return Planet.FaceRenderMask.All;
        }
    }
}
