using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    PlantVisualsController plantVisuals;

    public Dictionary<string, Plant> PlantPrototypes;

    public List<Plant> Plants;
    public List<Plant> PlantsToRemove;

    public void SetUp()
    {
        plantVisuals = FindObjectOfType<PlantVisualsController>();
        plantVisuals.SetUp();
        PlantPrototypes = new Dictionary<string, Plant>();
        Plants = new List<Plant>();

    }

    public Plant CreatePlant(string type, Vector3 position)
    {
        Plant proto = PlantPrototypes[type];

        if(proto == null)
        {
            Debug.Log("PlantProtos did not contain " + type);
            return null;
        }

        Plant plant = Plant.CreatePlant(proto, position);

        Plants.Add(plant);

        plantVisuals.CreatePlant(plant, position);

        return plant;
    }

    public void AgePlants(int days, Dictionary<Plant, bool> toAge)
    {
        List<Plant> plantsToChange = new List<Plant>();
        PlantsToRemove = new List<Plant>();
        
        foreach(Plant plant in toAge.Keys)
        {
            int stage = plant.GrowthStage;
            plant.GetOlder(days, toAge[plant]);
            if(plant.GrowthStage != stage)
            {
                plantsToChange.Add(plant);
            }

            if(plant.IsPlantDead() == true)
            {
                PlantsToRemove.Add(plant);
            }
        }

        foreach(Plant plant in plantsToChange)
        {
            plantVisuals.OnPlantChanged(plant);
        }
    }

    public void OnPlantChange(Plant plant)
    {
        plantVisuals.OnPlantChanged(plant);
    }

    public void OnPlantRemoved(Plant plant)
    {
        Plants.Remove(plant);
        plantVisuals.OnPlantRemoved(plant);
    }
}
