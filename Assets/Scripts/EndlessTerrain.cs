using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour {

    public const float maxViewDist = 450;
    public Transform viewer;

    public static Vector2 viewerPosition;
    static MapGenerator mapGenerator; 
    int chunkSize;
    int chunkVisibleInViewDist;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionart = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> tarrainChunkVisibleLastUpdate = new List<TerrainChunk>(); 

    private void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>(); 
        chunkSize = MapGenerator.mapChunkSize - 1;
        chunkVisibleInViewDist = Mathf.RoundToInt (maxViewDist / chunkSize);

    }
    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x,viewer.position.z);
        UpdateVisibleChunks(); 
    }

    void UpdateVisibleChunks()
    {
        for (int i = 0; i < tarrainChunkVisibleLastUpdate.Count; i++)
        {
            tarrainChunkVisibleLastUpdate[i].SetVisible(false); 

        }
        tarrainChunkVisibleLastUpdate.Clear(); 
        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int yOffset = -chunkVisibleInViewDist; yOffset <=chunkVisibleInViewDist; yOffset++)
        {
            for (int xOffset = -chunkVisibleInViewDist; xOffset <=chunkVisibleInViewDist; xOffset++)
            {
                Vector2 viewedChunkCoor = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (terrainChunkDictionart.ContainsKey(viewedChunkCoor))
                {
                    terrainChunkDictionart[viewedChunkCoor].UpdateTarrainChunk();
                    if (terrainChunkDictionart[viewedChunkCoor].IsVisible())
                    {
                        tarrainChunkVisibleLastUpdate.Add(terrainChunkDictionart[viewedChunkCoor]); 
                    }
                }
                else
                {
                    terrainChunkDictionart.Add(viewedChunkCoor, new TerrainChunk(viewedChunkCoor,chunkSize,transform)); 
                }

            }
        }
    }

    public class TerrainChunk
    {
        GameObject meshObject; 
        Vector2 position;
        Bounds bounds; 


        public TerrainChunk(Vector2 coord,int size,Transform parent)
        {
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size); 
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);
            meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            meshObject.transform.position = positionV3;
            meshObject.transform.localScale = Vector3.one * size/10f;
            meshObject.transform.parent = parent; 
            SetVisible(false); 
        }
        public void UpdateTarrainChunk()
        {
           float viewerDistFromNearestEdege=Mathf.Sqrt (bounds.SqrDistance(viewerPosition));
            bool visible = viewerDistFromNearestEdege <= maxViewDist;
            SetVisible(visible); 
        }


        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible); 
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf; 
        }
    }
}
