using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 
using System.Threading; 


public class MapGenerator : MonoBehaviour {

    public enum DrawMode
    {
        NoiseMap,ColourMap,Mesh
    };

    public DrawMode drawMode;
    
    public bool autoUpdate;
    public const int mapChunkSize = 241;
    [Range(0,6)]
    public int levelOfDetail; 

    public float noiseScale;
    //Height Multyplyer 
    public float meshHeightMultyplyer;
    public AnimationCurve meshHeightCurve; 
    //Noise map Variables techinicals 
    public int octives;
    [Range(0,1)]
    public float persistance;
    public float lacuranity;

    //Seed and its Offset
    public int seed;
    public Vector2 offsets;

    //Assinging Regions by decelaring Struct
    public TarrainTypes[] regions;

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>(); 

    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(); 
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(mapData.colorMap, mapChunkSize, mapChunkSize));

        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MesGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultyplyer, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColourMap(mapData.colorMap, mapChunkSize, mapChunkSize));
        }
    }

    public void RequestMapData(Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(callback);
        };

        new Thread(threadStart).Start(); 
    }

    public void MapDataThread(Action<MapData> callback)
    {
        MapData mapData = GenerateMapData();
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    private void Update()
    {
        if (mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo-mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter); 
            }
        }
    }
    public MapData GenerateMapData()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize,seed, noiseScale,octives,persistance,lacuranity, offsets);

        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];

        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapChunkSize + x] = regions[i].color; 
                        break; 
                    }
                }
            }
        }
        return new MapData(noiseMap, colourMap); 
       
    }

    //autometicaly checks on chage of variables 
    private void OnValidate()
    {
        //if (mapChunkSize < 1)
        //{
        //    mapChunkSize = 1; 
        //}
        //if (mapChunkSize < 1)
        //{
        //    mapChunkSize = 1; 
        //}

        //Lacanarity sould nave be less than 1 
        if (lacuranity < 1)
        {
            lacuranity = 1; 
        }
        //Octives cannot be nmegitive 
        if (octives < 0)
        {
            octives = 1; 
        }
    }

    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter; 
    }

}

[System.Serializable]
public struct TarrainTypes
{
    public string name;
    public float height;
    public Color color;
     
}

public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colorMap;
    public MapData(float[,] heightMap, Color[] colorMap)
    {
        this.heightMap = heightMap;
        this.colorMap = colorMap; 
    }
}; 