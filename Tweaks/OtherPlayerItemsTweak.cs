using System;
using System.Text;
using AmadareTweaks.Logging;
using RoR2;

namespace AmadareTweaks.Tweaks;

class OtherPlayerItemsTweak : Tweak
{
    private static readonly StringBuilder SharedStringBuilder = new();
    
    public override void Register()
    {
        On.RoR2.GenericPickupController.GetContextString += (orig, self, activator) =>
        {
            var text = orig(self, activator);
            if (!this.Config.DisplayOtherPlayerItemCount)
            {
                return text;
            }
            try
            {
                return GetOtherPlayersCount(ref self.pickupIndex, text);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return text;
            }
        };
    }
    
    private string GetOtherPlayersCount(ref PickupIndex pickupIndex, string origDescription)
    {
        var pickup = PickupCatalog.GetPickupDef(pickupIndex);
        
        // sometimes GetContextString returns previously returned value and I cannot find where it is cached
        // so I just making sure to trim part before my additions ¯\_(ツ)_/¯
        var sorry = origDescription.IndexOf("<size=90");
        SharedStringBuilder.Clear()
            .Append(origDescription, 0, sorry > 0 ? sorry : origDescription.Length)
            .Append("<size=90%>\n");
        
        foreach (var playerCharacterMasterController in PlayerCharacterMasterController.instances)
        {
            if (!this.Config.DisplayOwnItemCount && playerCharacterMasterController.networkUser.isLocalPlayer) continue;
            var itemCount = playerCharacterMasterController.master.inventory.GetItemCount(pickup.itemIndex);
            var displayName = playerCharacterMasterController.GetDisplayName();
            SharedStringBuilder.AppendFormat("<align=\"right\"><color=#bababa>{0}: </color><color=yellow>{1}</color>", displayName, itemCount);
        }

        return SharedStringBuilder.ToString();
    }
}