using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Northgard.GameWorld.Abstraction;
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
        var newTerritory = worldPipeline.InstantiateTerritory(territoryPrefab);
        newTerritory.SetPosition(Vector3.up);
        worldEditor.World.AddTerritory(newTerritory);
        var naturalDistrictPrefab = worldPipeline.NaturalDistrictPrefabs.First();
        var newNaturalDistrict =
            worldPipeline.InstantiateNaturalDistrict(naturalDistrictPrefab);
        newNaturalDistrict.SetPosition(Vector3.up * 1.5f);
        newTerritory.AddNaturalDistrict(newNaturalDistrict);
    }
}
