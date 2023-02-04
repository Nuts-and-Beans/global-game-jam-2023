using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PotionList
{
    private static StackArray<Potion> potionList;
    private const int maxPotionListSize = 50; 

    static PotionList()
    {
        GeneratePotionList();
    }

    private static void GeneratePotionList()
    {
        potionList = new(maxPotionListSize);
        
        for (int i = 0; i < maxPotionListSize; i++)
        {
            Potion potion = new Potion();
            potionList.Push(potion);
        }
    }
    
    public static Potion GetNextPotion() => potionList.Pop();
    public static void ReturnPotion(Potion potion) => potionList.Push(potion);
}
