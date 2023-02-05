// using System;
// using System.Collections;
// using System.Collections.Generic;
// using Random = UnityEngine.Random;
// using UnityEngine;
//
// public static class EquipmentList
// {
//     private static StackArray<Equipment> equipmentList;
//     private const int maxEquipmentListSize = 50; 
//
//     static EquipmentList()
//     {
//         GenerateEquipmentList();
//     }
//
//     private static void GenerateEquipmentList()
//     {
//         equipmentList = new(maxEquipmentListSize);
//         
//         for (int i = 0; i < maxEquipmentListSize; i++)
//         {
//             Equipment equipment = new Equipment();
//             int randomNum;
//             int bonusAmount;
//
//             randomNum = Random.Range(0, 5); // 5 types of equipment
//             equipment.equipmentType = (EquipmentType)randomNum;
//
//             // 3 is the max the armour will increase the stat by
//             randomNum = Random.Range(1, 4); 
//             bonusAmount = randomNum;
//             
//             randomNum = Random.Range(0, 3); // Which stat to increase
//             SetEquipmentBonus(equipment, bonusAmount, (StatType)randomNum);
//
//             equipmentList.Push(equipment);
//         }
//     }
//     
//     public static Equipment GetNextEquipment() => equipmentList.Pop();
//     public static void ReturnEquipment(Equipment equipment) => equipmentList.Push(equipment);
//     
//     private static void SetEquipmentBonus(Equipment equipment, int bonusAmount, StatType statType)
//     {
//         switch (statType)
//         {
//             case StatType.HEALTH:
//                 equipment.healthBonus = bonusAmount;
//                 break;
//             case StatType.ATTACK:
//                 equipment.attackBonus = bonusAmount;
//                 break;
//             case StatType.AGILITY:
//                 equipment.agilityBonus = bonusAmount;
//                 break;
//         }
//     }
//     
//     private enum StatType
//     {
//         HEALTH = 0,
//         ATTACK,
//         AGILITY
//     }
// }
