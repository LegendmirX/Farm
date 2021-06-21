using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCVisualsController : MonoBehaviour
{
    GameObject PlayerGO;

    Dictionary<NPC, GameObject> npcGoMap;

    public void Setup()
    {
        npcGoMap = new Dictionary<NPC, GameObject>();
    }

    public GameObject BuildPlayer(Player p, Vector3 position)
    {
        GameObject go = Instantiate(GameAssets.i.Character);
        go.transform.position = position;

        go.tag = "Player";

        Camera.main.transform.SetParent(go.transform, false);
        Camera.main.transform.position += new Vector3(0,10,0);

        PlayerGO = go;
        return go;
    }

    public GameObject BuildNPC(NPC c, Vector3 position)
    {
        GameObject go = Instantiate(GameAssets.i.Character);
        go.tag = "NPC";
        go.transform.position = position;

        npcGoMap.Add(c, go);

        return go;
    }
}
