using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise {

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed ,float scale, int octives, float persistance ,float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];
        
        // To keep track of max height and min height
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;
        //Centering the Map on noise scale change 
        float halfWidth = mapWidth / 2;
        float halfHeight = mapHeight / 2; 

        // Seed Randomizer 
        System.Random prng = new System.Random(seed);
            //octive offsets 
        Vector2[] octivesOffsets = new Vector2[octives];
        for (int i = 0; i < octives; i++)
        {
            float offsetX = prng.Next(-100000, 100000)+offset.x;
            float offsetY = prng.Next(-100000, 100000)+offset.y;
            octivesOffsets[i] = new Vector2(offsetX, offsetY); 
        }


        if (scale <= 0)
        {
            scale = 0.0001f;
        }


        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octives; i++)
                {

                    

                    float sampleX = (x-halfWidth) / scale*frequency+octivesOffsets[i].x;
                    float sampleY = (y-halfHeight) / scale*frequency+ octivesOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY)*2-1;//*2-1 was done to make it (-1,1)

                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity; 
     
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight; 
                }
                else if(noiseHeight<minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight; 
                }
                noiseMap[x, y] = noiseHeight; 
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                // Normaliziung our height map
                // Inverslerp- returns (0-1) if = min nous 0, max 1 middle 5 ; 
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]); 

            }
        }
                return noiseMap; 
    }

}
