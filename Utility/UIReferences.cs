using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIReferences : MonoBehaviour
{
    public static UIReferences i;

    [Space]
    [Header("ToolBar:")]
    public GameObject ToolBar;
    public GameObject ToolBarItemField;

    [Space]
    [Header("Inventory:")]
    public GameObject Inventory;
    public GameObject InventoryItemField;
    public GameObject ToolBarInvField;

    [Space]
    [Header("TradeInventory:")]
    public GameObject TradeInv;
    public GameObject TradeInvItemFieldA;
    public TextMeshProUGUI TradeTextA;
    public GameObject TradeInvItemFieldB;
    public TextMeshProUGUI TradeTextB;

    [Space]
    [Header("Clock")]
    public GameObject ClockTimeField;
    public GameObject ClockDayField;

    [Space]
    [Header("Debug")]
    public GameObject DebugMainMenu;

    [Space]
    [Header("NeedsUI")]
    public GameObject NeedsUI;
    public GameObject NeedsUI_NPCItemField;
    public GameObject NeedsNPCItem;
    public GameObject NeedItem;

    [Space]
    [Header("BuildUI")]
    public GameObject BuildMenu;
    public GameObject BuiltItemField;
    public GameObject BuildItem;

    [Space]
    [Header("TimeUI")]
    public GameObject TimeMenu;
    public InputField TimeDaysInput;
    public GameObject TimeDaysGO;
    public InputField TimeHoursInput;
    public GameObject TimeHoursGO;
    public InputField TimeMinsInput;
    public GameObject TimeMinsGO;

    [Space]
    [Header("Misc:")]
    public GameObject InGameUICanavas;
    public GameObject Slot;
    public GameObject Selection;
    public GameObject ItemDrag;

    public void SetUp()
    {
        i = this;

        ToolBarItemField = ToolBar.transform.Find("ItemField").gameObject;

        GameObject tradeBack    = TradeInv.transform.Find("InventoryBackImg").gameObject;
        TradeTextA              = tradeBack.transform.Find("TextA").GetComponent<TextMeshProUGUI>();
        TradeTextB              = tradeBack.transform.Find("TextB").GetComponent<TextMeshProUGUI>();
        TradeInvItemFieldA      = tradeBack.transform.Find("ItemFieldA").gameObject;
        TradeInvItemFieldB      = tradeBack.transform.Find("ItemFieldB").gameObject;

        TimeDaysInput   = TimeDaysGO.GetComponent<InputField>();
        TimeHoursInput  = TimeHoursGO.GetComponent<InputField>();
        TimeMinsInput   = TimeMinsGO.GetComponent<InputField>();
        
    }
}
