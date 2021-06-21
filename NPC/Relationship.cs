using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Relationship
{
    public enum RelationshipStatus
    {
        Adversary,
        Acquaintance,
        Friend,
        CloseFriend,
        LoveInterest,
        Lover
    }
    public RelationshipStatus Status;

    public float StatValue = 50;
    public int lastChatLine;

    private bool Interacted = false;
    public int DaysSinceLastInteraction = 0;
    private int normalizeAmount = 10;

    private int maxStatRange = 100;

    public void EndDay(int days)
    {
        if(Interacted == false)
        {
            DaysSinceLastInteraction++;
            
            if(DaysSinceLastInteraction >= 3)
            {
                Normalize();
            }
            return;
        }
        Interacted = false;
        DaysSinceLastInteraction = 0;
    }

    public void Normalize()
    {
        if(StatValue == 50)
        {
            return;
        }

        if(StatValue >= 50 + normalizeAmount)
        {
            StatValue -= normalizeAmount;
        }
        else if(StatValue > (50 - normalizeAmount) && StatValue < (StatValue + normalizeAmount))
        {
            StatValue = 50;
        }
        else if(StatValue <= (50 - normalizeAmount))
        {
            StatValue += normalizeAmount;
        }
    }

    public void ChangeRelationship(float value)
    {
        Interacted = true;
        StatValue += value;

        if(StatValue >= maxStatRange)
        {
            Status += 1;
            StatValue -= (maxStatRange - (maxStatRange/2));
            if((int)Status > Enum.GetValues(typeof(RelationshipStatus)).Length - 1)
            {
                Status = (RelationshipStatus)Enum.GetValues(typeof(RelationshipStatus)).Length - 1;
                StatValue = maxStatRange;
                return;
            }
            return;
        }

        if(StatValue <= 0)
        {
            Status -= 1;
            float diff = StatValue;
            StatValue = (maxStatRange + diff) - (maxStatRange/2);
            if ((int)Status < 0)
            {
                Status = 0;
                StatValue = 0;
            }
            return;
        }
    }


}
