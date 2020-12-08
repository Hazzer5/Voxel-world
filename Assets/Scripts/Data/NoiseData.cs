using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class NoiseData : ScriptableObject
{
    public int seed; // the seed for the random geneator
    public int octaves; //the number of times the perlin noise is added

    [Range(0, 1)]
    public float persistance; //how much the amplitude is multiplide by between octaves
    public float lacunarity; //how much the frequncy is multiplide by between octaves

    public float scale;

    public Vector2 offset;

    public AnimationCurve heightCurve;
    public float heightMultiplier;

    public void ValidateValues() {
        scale = Mathf.Max(scale, 0.01f);
        octaves = Mathf.Max(octaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1);
        persistance = Mathf.Clamp01(persistance);
    }    
}
