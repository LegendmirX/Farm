using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstalledObjectManager : MonoBehaviour
{
    public InstalledObjectVisuals installedObjectVisuals;

    public Dictionary<string, InstalledObject> InstalledObjectPrototypes;

    public List<InstalledObject> InstalledObjects;

    public void SetUp()
    {
        InstalledObjects = new List<InstalledObject>();
        installedObjectVisuals = FindObjectOfType<InstalledObjectVisuals>();
        installedObjectVisuals.SetUp();
    }

    public void UpdateFrame(float deltaTime)
    {
        foreach(InstalledObject obj in InstalledObjects)
        {
            obj.Update(deltaTime);
        }
    }

    public InstalledObject CreateInstalledObject(string type, Vector3 position)
    {
        InstalledObject proto = InstalledObjectPrototypes[type];

        if (proto == null)
        {
            Debug.Log("InstalledObjectProtos did not contain " + type);
            return null;
        }

        InstalledObject installedObject = InstalledObject.CreateInstalledObject(proto, position);

        InstalledObjects.Add(installedObject);

        GameObject GO = installedObjectVisuals.CreateInstalledObject(installedObject, position);

        installedObject.SetGO(GO);

        return installedObject;
    }

    public void OnInstalledObjectRemoved(InstalledObject obj)
    {
        InstalledObjects.Remove(obj);
        installedObjectVisuals.OnInstalledObjectRemoved(obj);
    }

    public void Interact(InstalledObject obj, Player player)
    {
        Interact(obj, player, null);
    }

    public void Interact(InstalledObject obj, NPC npc)
    {
        Interact(obj, null, npc);
    }

    private void Interact(InstalledObject obj, Player player = null, NPC npc = null)
    {
        obj.Interact(player, npc);
    }
}
