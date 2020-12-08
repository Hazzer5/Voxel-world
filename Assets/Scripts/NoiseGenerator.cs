using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator
{
    Vector2[] octaveOffsets;
    System.Random prng;
    float maxPossibleHeight;

    NoiseData noiseSettings;

    public NoiseGenerator(NoiseData noiseSettings, Vector2 sampleCenter) {
        this.noiseSettings = noiseSettings;
        prng = new System.Random(noiseSettings.seed);
        octaveOffsets = new Vector2[noiseSettings.octaves];

        maxPossibleHeight = 0;
        float amplitude = 1;

        for (int i = 0; i < noiseSettings.octaves; i++){
            float offsetX = prng.Next(-100000, 100000) + noiseSettings.offset.x + sampleCenter.x;
            float offsetY = prng.Next(-100000, 100000) + noiseSettings.offset.y + sampleCenter.y;

            octaveOffsets[i] = new Vector2 (offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= noiseSettings.persistance;
        }
    }

    public float GenerateNosieValue(int x, int y) {

        float amplitude = 1;
        float frequncy = 1;
        float noiseHeight = 0;

        for (int i = 0; i < noiseSettings.octaves; i++) {
            float sampleX = (x + octaveOffsets[i].x) / noiseSettings.scale * frequncy;
            float sampleY = (y + octaveOffsets[i].y) / noiseSettings.scale * frequncy;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1; //random value between -1, 1
            noiseHeight += perlinValue * amplitude;

            amplitude *= noiseSettings.persistance;
            frequncy *= noiseSettings.lacunarity;
        }

        
        float value = Mathf.InverseLerp(-maxPossibleHeight, maxPossibleHeight, noiseHeight); // gives it a value between 1 and 0

        return value;
    }

    public float GenerateNosieValue(int rawx, int rawy, int rawz) {
        int x = rawx + Mathf.FloorToInt(octaveOffsets[0].x);
        int y = rawy;
        int z = rawz + Mathf.FloorToInt(octaveOffsets[0].y);

        float xy = GenerateFixedNosieValue(x, y);
        float xz = GenerateFixedNosieValue(x, z);
        float yz = GenerateFixedNosieValue(y, z);
        float yx = GenerateFixedNosieValue(y, x);
        float zx = GenerateFixedNosieValue(z, x);
        float zy = GenerateFixedNosieValue(z, y);

        return (xy + xz +yz + yx + zx + zy) / 6.0f;
    }

    public float GenerateFixedNosieValue(int x, int y) {

        float amplitude = 1;
        float frequncy = 1;
        float noiseHeight = 0;

        for (int i = 0; i < noiseSettings.octaves; i++) {
            float sampleX = (x) / noiseSettings.scale * frequncy;
            float sampleY = (y) / noiseSettings.scale * frequncy;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1; //random value between -1, 1
            noiseHeight += perlinValue * amplitude;

            amplitude *= noiseSettings.persistance;
            frequncy *= noiseSettings.lacunarity;
        }

        
        float value = Mathf.InverseLerp(-maxPossibleHeight, maxPossibleHeight, noiseHeight); // gives it a value between 1 and 0

        return value;
    }
}
