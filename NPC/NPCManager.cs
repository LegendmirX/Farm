using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    NPCVisualsController npcVisuals;

    public Dictionary<string, NPC> CharacterPrototypes;

    public Player Player { get; protected set; }
    public List<NPC> NPCS;

    #region NeedsValues
    private int maxNeeds = 80;
    private int minNeeds = 30;
    #endregion

    public void DebugPathTest(Vector3 destination)
    {
        Debug.Log("PathTest");
        setNPCDestination(NPCS[0], destination);
    }

    public void SetUp()
    {
        npcVisuals = FindObjectOfType<NPCVisualsController>();
        npcVisuals.Setup();
        NPCS = new List<NPC>();
    }

    public void Updateframe(float deltaTime)
    {
        foreach(NPC npc in NPCS)
        {
            npc.Updateframe(deltaTime);

            if(npc.Distance > 0.9 && npc.Path == null) //Check if NPC needs a path.
            {
                npc.SetPath( WorldController.instance.currentArea.PathFindingGrid.FindPath(npc.Position, npc.Destination) );
            }

            if(npc.needs.InNeed == true && npc.Path == null) //Check if NPC needs somthing and isnt busy
            {
                if(npc.needs.InNeedOf == Needs.Need.Social)
                {
                    List<string> names = getPeopleILike(npc);

                    if(names != null)
                    {
                        foreach(string name in names)
                        {
                            Vector3 position = findPerson(name);
                            if(position != (Vector3.one / -1)) //find who we can get to
                            {
                                setNPCDestination(npc, position);
                                return;
                            }
                        }
                    }
                    return;
                }

                Vector3 destination = WorldController.instance.FindWhatINeed(npc.needs.InNeedOf, npc.Position);
                //TODO: Should we have an interact point for the obj instead?
                if(destination != (Vector3.one * -1))
                {
                    //Debug.Log("Vector3 "+ destination);
                    setNPCDestination(npc, destination);
                }

                //TODO: set a checked for need thing or a way to stop this all running every frame.
            }
        }
    }

    public GameObject BuildPlayerInstance(Vector3 position) //Create Player Visuals
    {
        Vector3 pos = position;
        GameObject go;

        if (pos != null)
        {
            go = npcVisuals.BuildPlayer(Player, pos);
        }
        else
        {
            Debug.Log("StartPos for player is null??");
            go = npcVisuals.BuildPlayer(Player, new Vector3(50, 0 , 50));
        }

        return go;
    }

    public Player BuildPlayer() //Create Player Script
    {
        Player p = new Player();

        this.Player = p;

        p.SetUp();

        return p;
    }

    public void CreateNPC(string name, Vector3 position) //Create an NPC
    {
        NPC proto = CharacterPrototypes[name];
        NPC dude = NPC.CreateNPC(proto);
        dude.RegisterInteractCallback(WorldController.instance.InteractWithGameObject);
        dude.RegisterUpdateWalkableCallback(updateWalkable);

        NPCS.Add(dude);

        GameObject go = npcVisuals.BuildNPC(dude, position);

        setNPCDestination(dude, position);
        dude.SetGO(go);

        dude.needs = new Needs(UnityEngine.Random.Range(minNeeds, maxNeeds), UnityEngine.Random.Range(minNeeds, maxNeeds), UnityEngine.Random.Range(minNeeds, maxNeeds), UnityEngine.Random.Range(minNeeds, maxNeeds), UnityEngine.Random.Range(minNeeds, maxNeeds));
    }

    public void HourlyUpdate(int hours) //Hourly Update for needs degrading/ others
    {
        foreach (NPC npc in NPCS)
        {
            npc.needs.UpdateHours(hours);
        }
    }

    public void EndDay(int days) //End day for relationship updates, needs degrade and sleeping??
    {
        foreach(NPC dude in NPCS)
        {
            dude.EndDay(days);
        }
    }

    public void PlayerInteract(NPC dude) //Interacting with an NPC
    {
        //TODO: Check if character likes what ever is going on

        Relationship relationship = null;
        string playerName = Player.Name;

        if (dude.Relationships.ContainsKey(playerName))
        {
            relationship = dude.Relationships[playerName];
        }

        if(relationship == null)
        {
            //I dont know you but now i have met you
            Relationship newRelation = new Relationship();
            newRelation.Status = Relationship.RelationshipStatus.Acquaintance;
            dude.Relationships.Add(playerName, newRelation);
            //Debug.Log("New Relationship formed");
        }
        else
        {
            //I Know you
            relationship.ChangeRelationship(10f);
            //Debug.Log("Stage: " + relationship.StatValue + "\n" + "Status: " + relationship.Status.ToString());
        }

        string chatLine = dude.Chat(playerName); //Get chat line

        TextPopUp.Create(chatLine, dude.Position + new Vector3(0,0,2)); //display chat line

    }

    public void NPCInteract(NPC targetNPC, NPC activeNPC) //NPC interacting with other NPCS
    {
        int value = 50;

        Relationship targetRelationship = null;
        Relationship activeRelationship = null;
        string targetName = targetNPC.Name;
        string activeName = activeNPC.Name;

        if (targetNPC.Relationships.ContainsKey(activeName))
        {
            targetRelationship = targetNPC.Relationships[activeName];
        }

        if (activeNPC.Relationships.ContainsKey(targetName))
        {
            activeRelationship = activeNPC.Relationships[targetName];
        }

        if (targetRelationship == null) //guy being talked too
        {
            //I dont know you but now i have met you
            Relationship newRelation = new Relationship();
            newRelation.Status = Relationship.RelationshipStatus.Acquaintance;
            targetNPC.Relationships.Add(activeName, newRelation);
            //Debug.Log("New Relationship formed");
        }
        else
        {
            //I Know you
            targetRelationship.ChangeRelationship(10f);
            //Debug.Log("Stage: " + relationship.StatValue + "\n" + "Status: " + relationship.Status.ToString());
        }

        if (activeRelationship == null) //guy who initiated conversation
        {
            //I dont know you but now i have met you
            Relationship newRelation = new Relationship();
            newRelation.Status = Relationship.RelationshipStatus.Acquaintance;
            activeNPC.Relationships.Add(targetName, newRelation);
            //Debug.Log("New Relationship formed");
        }
        else
        {
            //I Know you
            activeRelationship.ChangeRelationship(10f);
            //Debug.Log("Stage: " + relationship.StatValue + "\n" + "Status: " + relationship.Status.ToString());
        }

        targetNPC.needs.ChangeNeed(Needs.Need.Social, value);
        activeNPC.needs.ChangeNeed(Needs.Need.Social, value);

        string chatLine = targetNPC.Chat(activeName); //Get chat line

        TextPopUp.Create(chatLine, targetNPC.Position + new Vector3(0, 0, 2)); //display chat line
    }

    private List<string> getPeopleILike(NPC npc)
    {
        List<string> names = new List<string>();
        if (npc.Relationships.Count > 0)
        {
            foreach (string n in npc.Relationships.Keys)
            {
                if (npc.Relationships[n].Status > Relationship.RelationshipStatus.Adversary)
                {
                    names.Add(n);
                }
            }

            return names;
        }

        return null;
    }

    private Vector3 findPerson(string name)
    {
        if(Player.Name == name)
        {
            return WorldController.instance.playerController.PlayerPosition();
        }

        foreach(NPC npc in NPCS)
        {
            if(npc.Name == name)
            {
                return npc.Position;
            }
        }

        return (Vector3.one / -1);
    }

    private void setNPCDestination(NPC npc, Vector3 destination)
    {
        if(destination == (Vector3.one / -1))
        {
            return;
        }

        Vector3 d = new Vector3(destination.x, 0, destination.z);
        npc.SetDestination(d);
    }

    private void updateWalkable(NPC npc)
    {
        Vector3 currentPathNodePos = npc.CurrentPathNodePos;
        Tile previousTile = WorldController.instance.GetTileAtWorldCoord(currentPathNodePos);
        Tile tile = WorldController.instance.GetTileAtWorldCoord(npc.Position);
        Vector3 newPathNodePos = new Vector3(tile.X, 0, tile.Z);

        // maybe change this later. register an action maybe?
        if (WorldController.instance.currentArea.InstalledObjectMap.ContainsKey(WorldController.instance.GetTileAtWorldCoord(currentPathNodePos)) == true)
        {
            InstalledObject obj = WorldController.instance.currentArea.InstalledObjectMap[previousTile];
            if (obj != null && obj.Type == InstalledObject.ObjectType.Door)
            {
                obj.Paramaters["hasPassedOver"] = true;
                Debug.Log("HasPassedOver");
            }
        }

        
        WorldController.instance.currentArea.PathFindingGrid.SetIsWalkable(currentPathNodePos, true);
        WorldController.instance.currentArea.PathFindingGrid.SetIsWalkable(newPathNodePos, false);

        npc.CurrentPathNodePos = newPathNodePos;
    }
}
