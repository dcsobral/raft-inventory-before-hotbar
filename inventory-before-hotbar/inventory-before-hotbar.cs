using UnityEngine;
using HarmonyLib;
using System.Reflection;

public class InventoryBeforeHotbar : Mod
{
    public Harmony harmony;

    public void Start()
    {
        harmony = new Harmony("com.dcsobral.InventoryBeforeHotbar");
        harmony.PatchAll(Assembly.GetExecutingAssembly());

        Debug.Log("Mod inventory-before-hotbar has been loaded!");
    }

    public void OnModUnload()
    {
        Debug.Log("Mod inventory-before-hotbar has been unloaded!");
    }
}

[HarmonyPatch(typeof(Inventory), "FindSuitableSlot")]
public class HarmonyPatch_InventoryBeforeHotbar
{
    [HarmonyPrefix]
    static bool FindSuitableSlot(ref Slot __result, Inventory __instance, Item_Base stackableItem)
    {
        if (stackableItem == null)
        {
            return true;
        } else
        {
            // Most of this is a copy of Inventory#FindSuitableSLot
            // it needs to be kept up-to-date with Raft's
            bool isStackable = stackableItem.settings_Inventory.Stackable;
            Slot slot = null;
            foreach (Slot slot2 in __instance.allSlots)
            {
                // Changed slot condition from slot == null
                if (slot2.IsEmpty && slot2.active && (slot == null || slot.slotType == SlotType.Hotbar))
                {
                    slot = slot2;
                }

                if (isStackable && slot2.active && !slot2.StackIsFull() && !slot2.IsEmpty && slot2.itemInstance.UniqueIndex == stackableItem.UniqueIndex)
                {
                    // Changed from return slot2 to conform to method patching
                    __result = slot2;
                    return false;
                }
            }
            // Changed from return slot to conform to method patching
            __result = slot;
            return false;
        }
    }
}
