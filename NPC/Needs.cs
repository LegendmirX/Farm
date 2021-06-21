using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Needs 
{
    public enum Need
    {
        Thirst,
        Hunger,
        Toilet,
        Sleep,
        Social
    }

    private Dictionary<Need, int> needsDictionary;

    public bool InNeed = false;
    public Need InNeedOf;
    public bool UpdateInNeed = false;

    private int MaxNeedsValue = 100;
    private int needsDegrade = 10;

    public Needs(int hunger, int thirst, int toilet, int sleep, int social)
    {
        needsDictionary = new Dictionary<Need, int>();

        for (int i = 0; i < Enum.GetValues(typeof(Need)).Length - 1; i++)
        {
            needsDictionary.Add((Need)i, 0);
        }

        needsDictionary[Need.Hunger]    = hunger;
        needsDictionary[Need.Thirst]    = thirst;
        needsDictionary[Need.Toilet]    = toilet;
        needsDictionary[Need.Sleep]     = sleep;
        needsDictionary[Need.Social]    = social;
    }

    public void UpdateHours(int hours)
    {
        Dictionary<Need, int> tempDic = new Dictionary<Need, int>();

        foreach (Need need in needsDictionary.Keys)
        {
            tempDic.Add(need, Mathf.Clamp(needsDictionary[need] - (needsDegrade * hours), 0, MaxNeedsValue));
        }

        needsDictionary = tempDic;
        UpdateInNeed = true;
    }

    public void SetNeed(Need need, int value)
    {
        needsDictionary[need] = value;
        needsDictionary[need] = Mathf.Clamp(needsDictionary[need], 0, MaxNeedsValue);
        UpdateInNeed = true;
    }

    public void ChangeNeed(Need need, int value)
    {
        Debug.Log("Change Need" + need + "+" + value);
        needsDictionary[need] += value;
        needsDictionary[need] = Mathf.Clamp(needsDictionary[need], 0, MaxNeedsValue);
        UpdateInNeed = true;
    }

    public int GetNeedValue(Need need)
    {
        return needsDictionary[need];
    }

    public void UpdateInNeedOf()
    {
        int needLength = Enum.GetValues(typeof(Need)).Length;
        //Debug.Log("NeedLength " + needLength);
        int inNeed = needLength;

        foreach (Need need in needsDictionary.Keys)
        {
            if(needsDictionary[need] < (MaxNeedsValue / 2))  //is anything below half?
            {
                if((int)need < inNeed)
                {
                    inNeed = (int)need;
                }
            }
        }

        //Debug.Log("InNeed int " + inNeed);
        if(inNeed > (Enum.GetValues(typeof(Need)).Length - 1))
        {
            //We dont need anything
            //Debug.Log("Didnt need anything");
            InNeed = false;
            return;
        }
        //We need somthing
        Needs.Need n = (Need)inNeed;
        //Debug.Log("Need " + n);
        InNeed = true;
        InNeedOf = n;
    }
}
