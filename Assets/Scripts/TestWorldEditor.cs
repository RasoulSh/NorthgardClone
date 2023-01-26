using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Northgard.GameWorld.Abstraction;
using Northgard.GameWorld.Data;
using Northgard.GameWorld.Entities;
using UnityEngine;
using Zenject;

public class TestWorldEditor : MonoBehaviour
{
    [Inject] private IWorldEditorService worldEditor;
    [Inject] private IWorldPipelineService worldPipeline;
    private IEnumerable<World> worldPrefabs;
    private IEnumerable<Territory> territoryPrefabs;
    private IEnumerable<NaturalDistrict> naturalDistrictPrefabs; 

    private void Start()
    {
        worldPrefabs = worldPipeline.WorldPrefabs;
        territoryPrefabs = worldPipeline.TerritoryPrefabs;
        naturalDistrictPrefabs = worldPipeline.NaturalDistrictPrefabs;
    }

    [ContextMenu("SelectWorld")]
    private void SelectWorld()
    {
        worldPipeline.SetWorld(worldPrefabs.First());
        worldEditor.World = worldPipeline.World;
    }

    [ContextMenu("AddTerritory")]
    private void AddTerritory()
    {
        var territoryPrefab = worldPipeline.TerritoryPrefabs.First();
        var worldBounds = worldPipeline.World.Data.Bounds;
        var territoryBounds = territoryPrefab.Bounds;
        var territorySizeX = territoryBounds.size.x;
        var territorySizeZ = territoryBounds.size.z;
        var startPosition = worldBounds.min + territoryBounds.extents + (Vector3.up * (worldBounds.size.y + territoryBounds.size.y + 0.1f));
        var minX = startPosition.x;
        var minZ = startPosition.z;
        var maxX = worldBounds.extents.x;
        var maxZ = worldBounds.extents.z;
        var pos = startPosition;
        for (float x = minX; x < maxX; x += territorySizeX)
        {
            for (float z = minZ; z < maxZ; z += territorySizeZ)
            {
                pos.x = x;
                pos.z = z;
                var newTerritory = worldPipeline.InstantiateTerritory(territoryPrefab);
                newTerritory.SetPosition(pos);
                worldEditor.World.AddTerritory(newTerritory);   
                var naturalDistrictPrefab = worldPipeline.NaturalDistrictPrefabs.First();
                var newNaturalDistrict =
                    worldPipeline.InstantiateNaturalDistrict(naturalDistrictPrefab);
                newNaturalDistrict.SetPosition(pos + Vector3.up * newNaturalDistrict.Data.Bounds.extents.y);
                newTerritory.AddNaturalDistrict(newNaturalDistrict);
            }
        }

        var worldDataset = new WorldDataset()
        {
            world = worldPipeline.World.Data,
            territories = worldPipeline.Territories.Select(t => t.Data).ToList(),
            naturalDistricts = worldPipeline.NaturalDistricts.Select(nd => nd.Data).ToList()
        };
        var worldJson = JsonUtility.ToJson(worldDataset);
        PlayerPrefs.SetString("TestWorldData", worldJson);
    }

    [ContextMenu("LoadFromData")]
    private void LoadFromData()
    {
        var worldJson = PlayerPrefs.GetString("TestWorldData");
        var worldData = JsonUtility.FromJson<WorldDataset>(worldJson);
        worldPipeline.SetWorld(worldData.world);
        worldEditor.World = worldPipeline.World;
        foreach (var territory in worldData.territories)
        {
            worldPipeline.InstantiateTerritory(territory);
        }
        foreach (var naturalDistrict in worldData.naturalDistricts)
        {
            worldPipeline.InstantiateNaturalDistrict(naturalDistrict);
        }
        worldPipeline.Initialize();
    }
}
