using AmadareTweaks.Logging;
using On.RoR2.Artifacts;
using RoR2;

namespace AmadareTweaks.Tweaks;

public class CommandKeyAlwaysTweak : Tweak
{
    public override void Register()
    {
        On.RoR2.Artifacts.CommandArtifactManager.OnDropletHitGroundServer += CommandArtifactManagerOnOnDropletHitGroundServer;
    }

    private void CommandArtifactManagerOnOnDropletHitGroundServer(CommandArtifactManager.orig_OnDropletHitGroundServer orig, ref GenericPickupController.CreatePickupInfo createpickupinfo, ref bool shouldspawn)
    {
        var pickup = PickupCatalog.GetPickupDef(createpickupinfo.pickupIndex);
        if (this.Config.PreventArtifactKeyCommandReplacement && pickup.itemIndex == RoR2Content.Items.ArtifactKey.itemIndex)
        {
            Log.Info("Prevent Artifact Key Command replacement.");
            shouldspawn = true;
            return;
        }
        
        orig(ref createpickupinfo, ref shouldspawn);
    }
}