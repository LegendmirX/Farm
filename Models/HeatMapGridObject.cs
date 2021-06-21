using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatMapGridObject 
{
    private const int MIN = 0;
    private const int MAX = 100;

    private GridUtil<HeatMapGridObject> grid;
    private int x;
    private int z;
    private int value;
    
    public HeatMapGridObject(GridUtil<HeatMapGridObject> grid, int x, int z)
    {
        this.grid = grid;
        this.x = x;
        this.z = z;
    }

    public void AddValue( int addValue )
    {
        value += addValue;
        value = Mathf.Clamp(value, MIN, MAX);
        grid.TriggerGridObjectChanged(x, z);
    }

    public float GetValueNormalized()
    {
        return (float)value / MAX;
    }

    public override string ToString()
    {
        return value.ToString();
    }

}
