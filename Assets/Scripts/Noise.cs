using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise {

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings, Vector2 sampleCentre) {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        float maxPossibleHeight = 0;
        float amplitude = 1;

        System.Random prng = new System.Random(settings.seed);
        Vector2[] octavesOffset = new Vector2[settings.octaves];
        for(int i = 0; i < settings.octaves; i++) {
            float offsetX = prng.Next(-100000, 100000) + settings.offset.x + sampleCentre.x;
            float offsetY = prng.Next(-100000, 100000) - settings.offset.y - sampleCentre.y;
            octavesOffset[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= settings.persistance;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for(int y = 0; y < mapHeight; y++) {
            for(int x = 0; x < mapWidth; x++) {

                amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for(int o = 0; o < settings.octaves; o++) {

                    float sampleX = (x - halfWidth + octavesOffset[o].x) / settings.scale * frequency;
                    float sampleY = (y - halfHeight + octavesOffset[o].y) / settings.scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= settings.persistance;
                    frequency *= settings.lacunarity;
                }

                if(noiseHeight > maxLocalNoiseHeight) maxLocalNoiseHeight = noiseHeight;
                if(noiseHeight < minLocalNoiseHeight) minLocalNoiseHeight = noiseHeight;

                noiseMap[x, y] = noiseHeight;

                if(settings.normalizeMode == NormalizeMode.Global) {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        if(settings.normalizeMode == NormalizeMode.Local) {
            for(int y = 0; y < mapHeight; y++) {
                for(int x = 0; x < mapWidth; x++) {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);

                }
            }
        }

        return noiseMap;
    }

    public enum NormalizeMode {
        Local,
        Global
    }
}

[System.Serializable]
public class NoiseSettings {


    public Noise.NormalizeMode normalizeMode;

    public float scale = 50;

    public int octaves = 6;
    [Range(0, 1)]
    public float persistance = 0.5f;
    public float lacunarity = 2;

    public int seed;
    public Vector2 offset;

    public void ValidateValues() {
        scale = Mathf.Max(scale, 0.01f);
        octaves = Mathf.Max(octaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1);

        persistance = Mathf.Clamp01(persistance);
    }
}