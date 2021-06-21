using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatMapBoolManager : MonoBehaviour
{
    private GridUtil<bool> grid;
    private Mesh mesh;
    private bool updateMesh;

    public void SetUp(GridUtil<bool> grid)
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        this.grid = grid;

        UpdateHeatMap();

        grid.OnGridObjectChanged += Grid_OnGridValueChanged;
    }

    private void LateUpdate()
    {
        if (updateMesh == true)
        {
            updateMesh = false;
            UpdateHeatMap();
        }
    }

    private void Grid_OnGridValueChanged(object sender, GridUtil<bool>.OnGridObjectChangedArgs e)
    {
        updateMesh = true;
    }

    private void UpdateHeatMap()
    {
        MeshUtils.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out Vector3[] verticies, out Vector2[] uvs, out int[] triangles);

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int z = 0; z < grid.GetHeight(); z++)
            {
                int index = x * grid.GetHeight() + z;

                Vector3 quadSize = new Vector3(1, 1) * grid.GetCellSize();
                Vector3 gridPos = grid.GetWorldPosition(x, z);
                Vector3 pos = new Vector3(gridPos.x, gridPos.z, 0);

                bool gridValue = grid.GetGridObject(x, z);
                float gridValueNorm;
                if (gridValue == true)
                {
                    gridValueNorm = 1f;
                }
                else
                {
                    gridValueNorm = 0f;
                }
                Vector2 gridValueUV = new Vector2(gridValueNorm, 0f);
                MeshUtils.AddToMeshArrays(verticies, uvs, triangles, index, pos, 90f, quadSize, gridValueUV, gridValueUV);
            }
        }

        mesh.vertices = verticies;
        mesh.uv = uvs;
        mesh.triangles = triangles;
    }
}
