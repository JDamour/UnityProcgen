﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapVoxelMeshView : MonoBehaviour, IWorldMapView
{
  [SerializeField] 
  private TerrainTable _terrainTable;

  [SerializeField] 
  private WorldMapViewChunk _chunkPrefab;

  [SerializeField] 
  private int chunkWidth = 16;
  
  [SerializeField] 
  private int chunkHeight = 16;

  [SerializeField]  
  private WorldMapGenerator _generator;

  [SerializeField] 
  private bool edges = false;
  
  [SerializeField] 
  private float edgeThickness = 1f;
  
  private Dictionary<Vector2,WorldMapViewChunk> _chunks = new Dictionary<Vector2,WorldMapViewChunk>();
  
  [SerializeField]
  private List<WorldMapViewChunk> chunkList = new List<WorldMapViewChunk>();
  
  public void Start()
  {
    if (_generator != null)
    {
      
    }
  }

  private void OnDisable()
  {
    ClearChunks();
  }
  
  public void DisplayMap(WorldMapData mapData)
  {
    if (!enabled)
    {
      return;
    }

    var heightMap = mapData.GetLayer<HeightMapLayerData>().heightMap;
    int chunksWide = Mathf.CeilToInt(mapData.width / (float)chunkWidth);
    int chunksHigh = Mathf.CeilToInt(mapData.height / (float)chunkHeight);

    if (Application.isEditor)
    {
      ClearChunks();
    }
    
    for (var y = 0; y < chunksHigh; y++)
    {
      for (var x = 0; x < chunksWide; x++)
      {
        var chunk = GetChunk(new Vector2Int(x,y));
        var meshData = VoxelMeshUtility.CreateMeshData(heightMap,mapData.width,mapData.height,x,y,chunkWidth,chunkHeight,_terrainTable,edges,edgeThickness);
        chunk.SetMesh(meshData.CreateMesh());
      }
    }
  }

  private WorldMapViewChunk GetChunk(Vector2Int chunkPt)
  {
    WorldMapViewChunk chunk = null;
    if (_chunks.TryGetValue(chunkPt, out chunk))
    {
      return chunk;
    }
    //Create a new chunk
    var loc = transform.TransformPoint(new Vector3(chunkPt.x*chunkWidth,0,chunkPt.y*chunkHeight));
    chunk = Instantiate(_chunkPrefab, loc, Quaternion.identity, transform);
    _chunks.Add(chunkPt,chunk);
    chunkList.Add(chunk);
    return chunk;
  }

  private void ClearChunks()
  {
    foreach (var chunk in chunkList)
    {
      if (chunk == null)
      {
        continue;
      }
      if (Application.isEditor)
      {
        DestroyImmediate(chunk.gameObject);
      }
      else
      {
        Destroy(chunk.gameObject);
      }
    }
    chunkList.Clear();
    _chunks.Clear();
  }
  
}
