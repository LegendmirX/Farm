using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant
{
    public Vector3 Position { get; protected set; }

    public string PlantType { get; protected set; }
    public int Age { get; protected set; }
    public int Lifespan { get; protected set; }
    public int AdultGrowthTime { get; protected set; }
    public bool NeedsWater { get; protected set; }
    public int HarvestQuantity { get; protected set; }
    public bool Regrows { get; protected set; }
    public string Tool { get; protected set; }
    public int GrowthStage { get; protected set; }

    private int daysPerGrowthStage;
    private int growthStageTimer;
    private int survivalTimeWithoutWater = 2;
    private int daysWithoutWater = 0;


    #region BuildFuncs
    public Plant()
    {
        
    }

    protected Plant(Plant other)
    {
        this.PlantType          = other.PlantType;
        this.Lifespan           = other.Lifespan;
        this.AdultGrowthTime    = other.AdultGrowthTime;
        this.HarvestQuantity    = other.HarvestQuantity;
        this.NeedsWater         = other.NeedsWater;
        this.Regrows            = other.Regrows;
        this.Tool               = other.Tool;

        other.daysPerGrowthStage = AdultGrowthTime / 6;

    }

    virtual public Plant Clone()
    {
        return new Plant(this);
    }

    static public Plant CreatePrototype(string plantType, int lifespan, int adultGrowthTime, bool needsWater = true, int harvestQuantity = 2, bool regrows = false, string tool = null)
    {
        Plant plant             = new Plant();
        plant.PlantType         = plantType;
        plant.Lifespan          = lifespan;
        plant.AdultGrowthTime   = adultGrowthTime;
        plant.NeedsWater        = needsWater;
        plant.HarvestQuantity   = harvestQuantity;
        plant.Regrows           = regrows;
        plant.Tool              = tool;

        return plant;
    }
    #endregion

    static public Plant CreatePlant(Plant proto, Vector3 position, int age = 0, int growthStage = 1, int growthStageTimer = 0)
    {
        Plant plant = proto.Clone();
        plant.Position = position;
        plant.Age = age;
        plant.GrowthStage = growthStage;
        plant.growthStageTimer = growthStageTimer;

        return plant;
    }

    public void GetOlder(int days, bool watered)
    {
        //Debug.Log("Plant get older: watered = " + watered + " needswater = " + NeedsWater);
        if(NeedsWater == true && watered == true || NeedsWater == false)
        {
            Age += days;

            if(Age > Lifespan)
            {
                //TODO: Remove plants on death
                Debug.Log("PlantDead plz impliment removal code");
            }

            growthStageTimer += days;
            if(growthStageTimer >= daysPerGrowthStage && GrowthStage != 6)
            {
                GrowthStage++;
            }
        }
        else if (NeedsWater == true && watered == false)
        {
            daysWithoutWater++;
        }

    }

    public bool ReadyToHarvest()
    {
        if(GrowthStage == 6)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Harvest()
    {
        GrowthStage = 4;
    }

    public string GetPlantSpriteName()
    {
        string name = null;

        name = PlantType + GrowthStage;

        return name;
    }

    public bool IsPlantDead()
    {
        if(daysWithoutWater > survivalTimeWithoutWater)
        {
            return true;
        }
        if(Age > Lifespan)
        {
            return true;
        }
        return false;
    }

}
