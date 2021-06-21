using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NeedsUI
{
    private NPC dude;
    private Needs.Need need;
    private GameObject bar;
    private TextMeshProUGUI textMesh;


    public NeedsUI(NPC dude, Needs.Need need, GameObject go)
    {
        this.dude = dude;
        this.need = need;
        textMesh = go.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        textMesh.text = need.ToString();
        bar = go.transform.Find("Bar").Find("Percent").gameObject;
        UpdateBar();
    }

    public void UpdateBar()
    {
        int value = dude.needs.GetNeedValue(need);
        float percent = value / 100f;

        bar.transform.localScale = new Vector3(percent, 1, 1);
    }
}
