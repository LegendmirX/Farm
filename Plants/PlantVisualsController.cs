using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantVisualsController : MonoBehaviour
{
    Dictionary<Plant, GameObject> plantGOMap;
    
    public void SetUp()
    {
        plantGOMap = new Dictionary<Plant, GameObject>();
    }

    public void CreatePlant(Plant plant, Vector3 position)
    {
        GameObject plantGO = new GameObject(plant.PlantType);
        plantGO.transform.position = position;
        plantGO.transform.Rotate(90f, plantGO.transform.rotation.y, plantGO.transform.rotation.z, Space.World);

        SpriteRenderer sr = plantGO.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteManager.current.GetSprite(SpriteManager.SpriteCatagory.Plants, plant.GetPlantSpriteName());
        sr.sortingLayerName = "Plants";

        plantGOMap.Add(plant, plantGO);
    }

    public void OnPlantChanged(Plant plant)
    {
        GameObject GO = plantGOMap[plant];
        SpriteRenderer sr = GO.GetComponent<SpriteRenderer>();
        sr.sprite = SpriteManager.current.GetSprite(SpriteManager.SpriteCatagory.Plants, plant.GetPlantSpriteName());
    }

    public void OnPlantRemoved(Plant plant)
    {
        GameObject go = plantGOMap[plant];

        Destroy(go);
    }
}
