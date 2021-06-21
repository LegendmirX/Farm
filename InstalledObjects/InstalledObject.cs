using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstalledObject
{
    public enum ObjectType
    {
        InstalledObject,
        Storage,
        Door,
        Shop
    }
    public ObjectType Type;

    public string SubType { get; protected set; }
    public Vector3 Position { get; protected set; }
    public bool IsWalkable { get; protected set; }
    public GameObject GO { get; protected set; }

    private Action<InstalledObject, float> updateAction;
    public Dictionary<string, Func<InstalledObject, object>> Functions;
    public Dictionary<string, object> Paramaters;

    public bool SatisfiesNeed { get; protected set; }
    public Needs.Need NeedSatisfies { get; protected set; }
    private Action<Player, NPC> interact;

    #region BuildFuncs
    public InstalledObject()
    {

    }

    protected InstalledObject(InstalledObject other)
    {
        this.Type           = other.Type;
        this.SubType        = other.SubType;
        this.IsWalkable     = other.IsWalkable;
        this.NeedSatisfies  = other.NeedSatisfies;
        this.SatisfiesNeed  = other.SatisfiesNeed;

        if(other.Paramaters != null)
        {
            this.Paramaters = new Dictionary<string, object>(other.Paramaters);
        }
        if(other.Functions != null)
        {
            this.Functions = new Dictionary<string, Func<InstalledObject, object>>(other.Functions);
        }
        if(other.updateAction != null)
        {
            this.updateAction = (Action<InstalledObject, float>)other.updateAction.Clone();
        }
        if(other.interact != null)
        {
            this.interact = (Action<Player, NPC>)other.interact.Clone();
        }
    }

    virtual public InstalledObject Clone()
    {
        return new InstalledObject(this);
    }

    static public InstalledObject CreatePrototype(ObjectType type, string subType, bool isWalkable, Needs.Need satisfies = default, bool satisfiesNeed = false, Action<Player, NPC> interact = null, List<Action<InstalledObject, float>> updateActions = null, Dictionary<string, object> objParams = null, Dictionary<string, Func<InstalledObject, object>> objFuncs = null)
    {
        InstalledObject installedObject = new InstalledObject();
        installedObject.Paramaters      = new Dictionary<string, object>();
        installedObject.Type            = type;
        installedObject.SubType         = subType;
        installedObject.IsWalkable      = isWalkable;
        installedObject.NeedSatisfies   = satisfies;
        installedObject.SatisfiesNeed   = satisfiesNeed;
        installedObject.interact        = interact;
        if(updateActions != null)
        {
            foreach(Action<InstalledObject,float> func in updateActions)
            {
                installedObject.updateAction += func;
            }
        }

        installedObject.Paramaters = objParams;
        installedObject.Functions = objFuncs;
        
        return installedObject;
    }
    #endregion

    static public InstalledObject CreateInstalledObject(InstalledObject proto, Vector3 position)
    {
        InstalledObject installedObject = proto.Clone();
        installedObject.Position = position;

        return installedObject;
    }

    public void Update(float deltaTime)
    {

        if(updateAction != null)
        {
            updateAction(this, deltaTime);
        }
    }

    public void SetGO(GameObject GO)
    {
        this.GO = GO;
    }
    
    public void Interact(Player player = null, NPC npc = null)
    {
        interact.Invoke(player, npc);
    }

    #region CallBacks
    public void RegisterUpdateAction(Action<InstalledObject, float> func)
    {
        updateAction += func;
    }
    public void UnregisterUpdateAction(Action<InstalledObject, float> func)
    {
        updateAction -= func;
    }
    #endregion
}
