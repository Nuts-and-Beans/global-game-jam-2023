using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion
{
    public PotionType potionType;
    private int healAmount = 1;
    
    
}

public enum PotionType
{
    HEALTH,
    TAKEDAMAGE,
    
}