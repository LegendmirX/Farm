using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;
using TMPro;

public class DebugFuncs : MonoBehaviour
{
    List<GameObject> NeedsUI_NPCsGOs;
    Dictionary<string, Dictionary<Needs.Need, NeedsUI>> NPCneeds;

    List<GameObject> BuildMenuGOs;

    public void ShowNPC_Needs()
    {
        if (UIReferences.i.NeedsUI.activeSelf == true)
        {
            UIReferences.i.NeedsUI.SetActive(false);
            foreach (GameObject go in NeedsUI_NPCsGOs)
            {
                Destroy(go);
            }
            return;
        }

        UIReferences.i.NeedsUI.SetActive(true);
        NeedsUI_NPCsGOs = new List<GameObject>();
        NPCneeds = new Dictionary<string, Dictionary<Needs.Need, NeedsUI>>();

        foreach (NPC NPC in WorldController.instance.npcManager.NPCS)
        {
            GameObject go = Instantiate(UIReferences.i.NeedsNPCItem, UIReferences.i.NeedsUI_NPCItemField.transform);
            go.transform.Find("Text").GetComponent<UnityEngine.UI.Text>().text = NPC.Name;
            NeedsUI_NPCsGOs.Add(go);

            Dictionary<Needs.Need, NeedsUI> needs = new Dictionary<Needs.Need, NeedsUI>();

            for (int i = 0; i < Enum.GetValues(typeof(Needs.Need)).Length; i++)
            {
                GameObject needGO = Instantiate(UIReferences.i.NeedItem, go.transform.Find("ItemField"));
                Needs.Need need = (Needs.Need)i;
                NeedsUI needUI = new NeedsUI(NPC, need, needGO);

                needs.Add(need, needUI);
            }

            NPCneeds.Add(NPC.Name, needs);
        }
    }

    public void UpdateNeeds()
    {
        if(UIReferences.i.NeedsUI.activeSelf == true)
        {
            foreach(string npc in NPCneeds.Keys)
            {
                Dictionary<Needs.Need, NeedsUI> npcNeeds = NPCneeds[npc];

                foreach(Needs.Need need in npcNeeds.Keys)
                {
                    npcNeeds[need].UpdateBar();
                }
            }
        }
    }

    public void BuildMenu()
    {
        BuildMenuGOs = new List<GameObject>();

        if(UIReferences.i.BuildMenu.activeSelf == true)
        {
            UIReferences.i.BuildMenu.SetActive(false);
            foreach (GameObject go in BuildMenuGOs)
            {
                Destroy(go);
            }
            return;
        }

        UIReferences.i.BuildMenu.SetActive(true);

        foreach(string s in WorldController.instance.installedObjectManager.InstalledObjectPrototypes.Keys)
        {
            GameObject go = Instantiate(UIReferences.i.BuildItem, UIReferences.i.BuiltItemField.transform);
            go.name = s + "-btn";
            go.GetComponentInChildren<Text>().text = s;
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener( () => ButtonFunc(s) );
            BuildMenuGOs.Add(go);
        }

    }

    private void ButtonFunc(string s)
    {
        WorldController.instance.mouseManager.currBuildItem = s;
    }

    public void ActiveTimePanel()
    {
        if(UIReferences.i.TimeMenu.activeSelf == true)
        {
            UIReferences.i.TimeMenu.SetActive(false);
        }
        else
        {
            UIReferences.i.TimeMenu.SetActive(true);
        }
    }

    public void AddMins()
    {
        int value = int.Parse(UIReferences.i.TimeMinsInput.text);
        WorldController.instance.IncrimentMins(value);
        WorldController.instance.UpdateClock();
    }

    public void AddHours()
    {
        int value = int.Parse(UIReferences.i.TimeHoursInput.text);
        WorldController.instance.IncrimentHours(value);
        WorldController.instance.UpdateClock();
    }

    public void AddDays()
    {
        int value = int.Parse(UIReferences.i.TimeDaysInput.text);
        WorldController.instance.IncrimentDays(value);
        WorldController.instance.UpdateClock();
    }

    public void DOTSPathFindPerformanceTest()
    {
        WorldController.instance.pathfindingDOTS.FindPathTest(0, 0, 99, 99, 5);
    }

    public void PathFindPerformanceTest()
    {
        float startTime = Time.realtimeSinceStartup;

        int pathTestCount = 5;
        for (int i = 0; i < pathTestCount; i++)
        {
            WorldController.instance.currentArea.PathFindingGrid.FindPath(0, 0, 99, 99);
        }
        Debug.Log((Time.realtimeSinceStartup - startTime) * 1000f);
    }
}
