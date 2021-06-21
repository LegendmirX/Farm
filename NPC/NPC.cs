using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC
{
    public string Name;
    public GameObject GO { get; protected set; }
    private Rigidbody rb;
    private Animator animator;
    private float walkSpeed;

    private Action<GameObject, NPC> interact;
    private Action<NPC> updateWalkable;

    public Vector3 Position
    {
        get { return GO.transform.position; }
        protected set { }
    }

    public List<PathNode> Path { get; protected set; }
    private Vector3 currentNode;
    public Vector3 Destination { get; protected set; }
    private Vector3 currentPosition;
    private Vector3 previousPosition;
    public Vector3 CurrentPathNodePos;
    public float Distance { get; protected set; }
    private float walkTime;
    private float takingTooLong = 3f;
    private Vector3 direction;

    public Inventory Inventory;

    public Dictionary<string, Relationship> Relationships;
    public Needs needs;

    private List<string> adversary;
    private List<string> acquaintance;
    private List<string> friend;
    private List<string> closeFriend;
    private List<string> loveInterest;
    private List<string> love;

    #region BuildFuncs
    public NPC()
    {

    }

    protected NPC(NPC other)
    {
        this.Name           = other.Name;
        this.walkSpeed      = other.walkSpeed;
        this.adversary      = new List<string>(other.adversary);
        this.acquaintance   = new List<string>(other.acquaintance);
        this.friend         = new List<string>(other.friend);
        this.closeFriend    = new List<string>(other.closeFriend);
        this.loveInterest   = new List<string>(other.loveInterest);
        this.love           = new List<string>(other.love);

        this.Relationships = new Dictionary<string, Relationship>();
    }

    virtual public NPC Clone()
    {
        return new NPC(this);
    }

    static public NPC CreatePrototype(string name, Dictionary<string, Relationship.RelationshipStatus> coversationStrings, float walkSpeed = 50)
    {
        NPC script = new NPC();
        script.Name = name;
        SortConverstationStrings(script, coversationStrings);

        script.walkSpeed = walkSpeed;

        return script;
    }

    private static void SortConverstationStrings(NPC script, Dictionary<string, Relationship.RelationshipStatus> coversationStrings)
    {
        script.adversary    = new List<string>();
        script.acquaintance = new List<string>();
        script.friend       = new List<string>();
        script.closeFriend  = new List<string>();
        script.loveInterest = new List<string>();
        script.love         = new List<string>();

        foreach (string s in coversationStrings.Keys)
        {
            switch (coversationStrings[s])
            {
                case Relationship.RelationshipStatus.Adversary:
                    script.adversary.Add(s);
                    break;
                case Relationship.RelationshipStatus.Acquaintance:
                    script.acquaintance.Add(s);
                    break;
                case Relationship.RelationshipStatus.Friend:
                    script.friend.Add(s);
                    break;
                case Relationship.RelationshipStatus.CloseFriend:
                    script.closeFriend.Add(s);
                    break;
                case Relationship.RelationshipStatus.LoveInterest:
                    script.loveInterest.Add(s);
                    break;
                case Relationship.RelationshipStatus.Lover:
                    script.love.Add(s);
                    break;
            }
        }
    }
    #endregion

    static public NPC CreateNPC(NPC proto)
    {
        NPC dude = proto.Clone();

        return dude;
    }

    public void Updateframe(float deltaTime)
    {
        currentPosition = Position;

        if(needs.UpdateInNeed == true) //Keep our needs uptodate
        {
            needs.UpdateInNeed = false;
            needs.UpdateInNeedOf();
        }

        if(Path == null || Path.Count == 0) //Got no where to go. Standing still.
        {
            Distance = Vector3.Distance(Position, Destination);
            Move(Vector3.zero, deltaTime);
            UpdateAnimator();
            return;
        }
        //Get moving
        Distance = Vector3.Distance(Position, currentNode);
        
        UpdateMovement(deltaTime);
        UpdateAnimator();
        previousPosition = GO.transform.position;
    }

    #region UpdateFuncs
    private void UpdateMovement(float deltaTime)
    {
        walkTime += deltaTime;

        if (Distance < 0.1)
        {
            walkTime = 0;
            if(Path != null && Path.Count > 0)
            {
                GetNextNode();
                updateWalkable(this);
            }
            return;
        }
        else if (Distance > 1.6 || walkTime >= takingTooLong)
        {
            Debug.Log("Pathing error, maybe player pushed me");
            Path = null;
            walkTime = 0;
            
            direction = CurrentPathNodePos - Position; //go back a node
            direction.Normalize();

            Move(direction, deltaTime);
            return;
        }

        direction = currentNode - Position;
        direction.Normalize();

        Move(direction, deltaTime);
    }

    private void UpdateAnimator()
    {
        animator.SetFloat("X", direction.x);
        animator.SetFloat("Z", direction.z);

        float speed = 0f;
        if(rb.velocity != Vector3.zero)
        {
            speed = walkSpeed;
        }

        animator.SetFloat("Speed",  speed);
    }
    #endregion

    private void Move(Vector3 direction, float deltaTime)
    {
        if(direction == Vector3.zero)// if direction is zero then we dont need to move.
        {
            rb.velocity = Vector3.zero;
            return;
        }

        PathNode.Enterability enterability = Path[0].IsEnterable();

        switch (enterability)
        {
            case PathNode.Enterability.Yes: ///GO GO GO
                rb.velocity = (direction * walkSpeed) * deltaTime;
                break;
            case PathNode.Enterability.Never: //never gonna get in to this tile
                Path = null;
                rb.velocity = Vector3.zero;
                break;
            case PathNode.Enterability.Soon: //waiting for entry
                walkTime = 0;
                rb.velocity = Vector3.zero;
                break;
        }
    }

    private void GetNextNode()
    {
        if(Path.Count > 1) //still nodes
        {
            Path.Remove(Path[0]);
            currentNode = new Vector3(Path[0].x, 0, Path[0].z);
        }
        else //out of path node
        {
            Path = null;
            Destination = Position;
            CheckForInteract();
            return;
        }

    }

    private void CheckForInteract()
    {
        RaycastHit hit;
        if(Physics.Raycast(Position, direction, out hit, 1f)) //just try the direction we are looking first
        {
            string tag = hit.transform.tag;
            if (tag == "PC" || tag == "Interactable" || tag == "NPC")
            {
                interact(hit.transform.gameObject, this);
                return;
            }
        }

        Vector3[] sides = new Vector3[4];
        sides[0] = new Vector3(0, 0, 1);
        sides[1] = new Vector3(1, 0, 0);
        sides[2] = new Vector3(0, 0, -1);
        sides[3] = new Vector3(-1, 0, 0);

        foreach(Vector3 directionCheck in sides)
        {
            if (Physics.Raycast(Position, directionCheck, out hit, 1f))
            {
                string tag = hit.transform.tag;
                if(tag == "PC" || tag == "Interactable" || tag == "NPC")
                {
                    interact(hit.transform.gameObject, this);
                    return;
                }
            }
        }
    }

    public void SetPath(List<PathNode> path)
    {
        if(path == null)
        {
            Debug.Log("Null Path what do?");
            return;
        }
        Path = path;

        if(currentNode == Path[0].Position)
        {
            Path.Remove(Path[0]);
        }

        currentNode = new Vector3(Path[0].x, 0, Path[0].z);
    }

    public void SetDestination(Vector3 destination)
    {
        Destination = destination;
    }

    public void SetGO(GameObject go)
    {
        GO = go;
        rb = GO.GetComponent<Rigidbody>();
        animator = go.GetComponentInChildren<Animator>();
        Destination = go.transform.position;
    }

    public void EndDay(int days)
    {
        foreach(string dude in Relationships.Keys) //Update relationships
        {
            Relationships[dude].EndDay(days);
        }

        //TODO:
        //Make sure we are in bed or somthing/somwhere we can sleep
        //Update needs to show we have rested the night

    }

    public string Chat(string name)
    {
        Relationship relationship = Relationships[name];
        string s = "";
        switch (relationship.Status) //Get appropriate chat line based on our relationship level
        {
            case Relationship.RelationshipStatus.Adversary:
                s = adversary[relationship.lastChatLine];
                SetChatLine(relationship, adversary.Count);
                break;
            case Relationship.RelationshipStatus.Acquaintance:
                s = acquaintance[relationship.lastChatLine];
                SetChatLine(relationship, acquaintance.Count);
                break;
            case Relationship.RelationshipStatus.Friend:
                s = friend[relationship.lastChatLine];
                SetChatLine(relationship, friend.Count);
                break;
            case Relationship.RelationshipStatus.CloseFriend:
                s = closeFriend[relationship.lastChatLine];
                SetChatLine(relationship, closeFriend.Count);
                break;
            case Relationship.RelationshipStatus.LoveInterest:
                s = loveInterest[relationship.lastChatLine];
                SetChatLine(relationship, loveInterest.Count);
                break;
            case Relationship.RelationshipStatus.Lover:
                s = love[relationship.lastChatLine];
                SetChatLine(relationship, love.Count);
                break;
        }

        //TODO: when traits are around maybe base somthing off traits here
        return s;
    }

    private void SetChatLine(Relationship r, int count) //Make sure we dont just say the same line over and over
    {
        r.lastChatLine -= 1;
        if (r.lastChatLine < 0)
        {
            r.lastChatLine = count-1;
        }
    }

    #region Callbacks
    public void RegisterInteractCallback(Action<GameObject, NPC> func)
    {
        interact += func;
    }
    public void UnregisterInteractCallback(Action<GameObject, NPC> func)
    {
        interact -= func;
    }

    public void RegisterUpdateWalkableCallback(Action<NPC> func)
    {
        updateWalkable += func;
    }
    public void UnregisterUpdateWalkableCallback(Action<NPC> func)
    {
        updateWalkable -= func;
    }
    #endregion

}
