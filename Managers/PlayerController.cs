using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class PlayerController : MonoBehaviour
{
    public Player PlayerData;
    public GameObject PlayerGO;
    public Animator Animator;
    public Rigidbody RB;

    public float MovementSpeedMultiplier = 1.0f;
    public Vector3 MovementInput;
    public float Speed;

    private Vector3 currentPosition;
    private Vector3 previousPosition;
    private Vector3 currentPathNodePos;

    GameObject selectionObj;

    protected int _selection;
    int Selection
    { get { return _selection; }
        set
        {
            _selection = value;

            if (selectionObj == null)
            {
                selectionObj = Instantiate(UIReferences.i.Selection);
            }

            selectionObj.transform.SetParent(UIReferences.i.ToolBarItemField.transform.GetChild(_selection - 1), false);
        }
    }

    //input controlls here.
    private void Update()
    {
        if(PlayerGO == null)
        {
            return;
        }

        currentPosition = PlayerGO.transform.position;

        GetInput();
        UpdateAnimator();
        RB.velocity = MovementInput * Speed * MovementSpeedMultiplier;
        UpdateWalkable();

        previousPosition = currentPosition;
    }

    #region UpdateFuncs
    public void GetInput()
    {
        #region Movement
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        if (movement != Vector3.zero)
        {
            MovementInput = movement;
        }
        Speed = Mathf.Clamp01(movement.magnitude);
        MovementInput.Normalize();
        #endregion

        if (Input.GetKeyDown(KeyCode.E)) //Interact with thing
        {
            RaycastHit hit;
            if(Physics.Raycast(PlayerPosition(), MovementInput, out hit, 1.25f))
            {
                //Debug.Log(hit.transform.name);
                GameObject go = hit.transform.gameObject;

                WorldController.instance.InteractWithGameObject(go, PlayerData);
            }
            Debug.DrawRay(PlayerPosition(), MovementInput, Color.red, 1f);
        }

        if (Input.GetKeyDown(KeyCode.I)) //OpenInv
        {
            if(WorldController.instance.inventoryManager.isInvOpen == false)
            {
                WorldController.instance.inventoryManager.OpenInventory(PlayerData.inventory);
                UIReferences.i.ToolBar.SetActive(false);
            }
            else
            {
                WorldController.instance.inventoryManager.CloseInventory();
                UIReferences.i.ToolBar.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.H)) //Harvest
        {
            Tile tile = InventoryItemActions.GetTileInFrontOfPlayer(); //Might change in future 
            WorldController.instance.Harvest(tile);
        }

        if (Input.GetKeyDown(KeyCode.Space))//UseSelectedItem
        {
            WorldController.instance.inventoryManager.UseToolBarItem(Selection - 1);
        }

        #region NumberPress
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Selection = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Selection = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Selection = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Selection = 4;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Selection = 5;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Selection = 6;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            Selection = 7;
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            Selection = 8;
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Selection = 9;
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Selection = 10;
        }
        #endregion
    }

    private void UpdateWalkable()
    {
        if (previousPosition != currentPosition)
        {
            if (Vector3.Distance(currentPosition, currentPathNodePos) > 0.5)
            {
                Tile tile = WorldController.instance.GetTileAtWorldCoord(currentPosition);

                Vector3 newPathNode = new Vector3(tile.X, 0, tile.Z);
                WorldController.instance.currentArea.PathFindingGrid.SetIsWalkable(currentPathNodePos, true);
                WorldController.instance.currentArea.PathFindingGrid.SetIsWalkable(newPathNode, false);

                currentPathNodePos = newPathNode;
            }
        }
    }

    private void UpdateAnimator()
    {
        Animator.SetFloat("X", MovementInput.x);
        Animator.SetFloat("Z", MovementInput.z);
        Animator.SetFloat("Speed", Speed);
    }
    #endregion

    #region HelperFuncs
    public void SetPlayer(Player p, GameObject go)
    {
        this.PlayerData = p;
        this.PlayerGO = go;
        Animator = go.GetComponentInChildren<Animator>();
        RB = go.GetComponent<Rigidbody>();
    }

    public Vector3 PlayerPosition()
    {
        return PlayerGO.transform.position;
    }
    #endregion
}
