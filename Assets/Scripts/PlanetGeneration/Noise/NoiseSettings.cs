using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseSettings
{
    public enum FilterType { Simple, Ridgid };
    public FilterType filterType;

    [ConditionalHide("filterType", 0)]
    public SimpleNoiseSettings simpleNoiseSettings;
    [ConditionalHide("filterType", 1)]
    public RidgidNoiseSettings ridgidNoiseSettings;

    [System.Serializable]
    public class SimpleNoiseSettings
    {
        public float strenght = 1;
        [Range(1, 8)]
        public int numberOfLayers = 1;
        public float baseRoughness = 1;
        public float roughness = 2;
        public float persistance = .5f; // amplitude of each layer will be effected by amout of persistence (0.5 halved)
        public Vector3 centre;
        public float minValue;
    }

    [System.Serializable]
    public class RidgidNoiseSettings : SimpleNoiseSettings
    {
        public float weightMultyplier = .8f;
    }
}
