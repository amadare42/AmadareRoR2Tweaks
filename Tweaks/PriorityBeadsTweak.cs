using AmadareTweaks.Logging;
using RoR2;

namespace AmadareTweaks.Tweaks;

public class PriorityBeadsTweak : Tweak
{
    public override void Register()
    {
        Log.Info($"Registering PriorityBeadsTweak");
        On.RoR2.CostTypeCatalog.LunarItemOrEquipmentCostTypeHelper.PayCost += (orig, def, context) =>
        {
            if (this.Config.PayBeadsChance == 0)
            {
                orig(def, context);
                return;
            }
            
            var inventory = context.activator.GetComponent<CharacterBody>().inventory;
            var beadsCount = inventory.GetItemCount(RoR2Content.Items.LunarTrinket);
            for (var i = 0; i < beadsCount; i++)
            {
                var roll = context.rng.nextNormalizedFloat;
                Log.Info($"Rolled {roll}");
                if (roll > this.Config.PayBeadsChance)
                {
                    inventory.RemoveItem(RoR2Content.Items.LunarTrinket);
                    context.results.itemsTaken.Add(RoR2Content.Items.LunarTrinket.itemIndex);
                    Log.Info($"Beads were prioritized after {i + 1} rolls!");
                    Chat.AddMessage($"Beads were picked after {i + 1} rolls!");
                    return;
                }
            }
            
            orig(def, context);
        };
    }
}