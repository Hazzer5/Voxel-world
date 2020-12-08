using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapGenerator
{

    //function will be expanded to do diferent things based on biome.
    public static HeightMap GenerateHeightMap(int width, int depth, Vector2 sampleCenter, NoiseData noiseSettings) {
        NoiseGenerator noiseGenerator = new NoiseGenerator(noiseSettings, sampleCenter);
        float[,] values = new float[width, depth];


        AnimationCurve heightCurve_threadSafe = new AnimationCurve(noiseSettings.heightCurve.keys);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                values[i, j] = Mathf.Clamp01(heightCurve_threadSafe.Evaluate(noiseGenerator.GenerateNosieValue(i, j))); 
            }
        }
        return new HeightMap(values);
    }


    
}

public struct HeightMap {
    public readonly float[,] values;

    public HeightMap(float[,] values)
    {
        this.values = values;
    }
}