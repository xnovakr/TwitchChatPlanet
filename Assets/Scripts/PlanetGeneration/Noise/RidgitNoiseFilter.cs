using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidgitNoiseFilter : INoiseFilter
{
    NoiseSettings.RidgidNoiseSettings settings;
    Noise noise = new Noise();

    public RidgitNoiseFilter(NoiseSettings.RidgidNoiseSettings settings)
    {
        this.settings = settings;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0;
        float frequency = settings.baseRoughness;
        float amplitude = 1;
        float weihgt = 1;

        for (int i = 0; i < settings.numberOfLayers; i++)
        {
            float v = 1 - Mathf.Abs(noise.Evaluate(point * frequency + settings.centre));
            v *= v;
            v *= weihgt;
            weihgt = Mathf.Clamp01(v * settings.weightMultyplier);

            noiseValue += v * amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistance;
        }
        noiseValue = noiseValue - settings.minValue;
        return noiseValue * settings.strenght;
    }
}
