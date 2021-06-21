using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstalledObjectVisuals : MonoBehaviour
{
    Dictionary<InstalledObject, GameObject> installedObjectGOMap;

    public void SetUp()
    {
        installedObjectGOMap = new Dictionary<InstalledObject, GameObject>();
    }

    public GameObject CreateInstalledObject(InstalledObject obj, Vector3 position)
    {
        GameObject GO = new GameObject(obj.SubType);
        GO.transform.position = position;
        GO.transform.Rotate(90f, GO.transform.rotation.y, GO.transform.rotation.z, Space.World);

        if(obj.SatisfiesNeed == true)
        {
            GO.tag = "Interactable";
        }
        else
        {
            switch (obj.Type)
            {
                case InstalledObject.ObjectType.Storage:
                    GO.tag = "Interactable";
                    break;
                case InstalledObject.ObjectType.Shop:
                    GO.tag = "Shop";
                    break;
                case InstalledObject.ObjectType.InstalledObject:
                    GO.tag = "InstalledObject";
                    break;
                case InstalledObject.ObjectType.Door:
                    GO.tag = "InstalledObject";
                    break;
            }
        }

        SpriteRenderer sr = GO.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteManager.current.GetSprite(SpriteManager.SpriteCatagory.InstalledObjects, obj.SubType);
        sr.sortingLayerName = "InstalledObjects";

        if(obj.IsWalkable == false)
        {
            BoxCollider col = GO.AddComponent<BoxCollider>();
        }

        installedObjectGOMap.Add(obj, GO);
        return GO;
    }

    public void OnInstalledObjectRemoved(InstalledObject obj)
    {
        GameObject go = installedObjectGOMap[obj];
        Destroy(go);
        installedObjectGOMap.Remove(obj);
    }
}
