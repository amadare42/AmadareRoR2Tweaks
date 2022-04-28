using AmadareTweaks.Logging;
using RoR2;

namespace AmadareTweaks.Tweaks;

public class MadDirectorTweak : Tweak
{
    public override void Register()
    {
        On.RoR2.SceneDirector.Start += (orig, self) =>
        {
            SceneInfo.instance.GetComponent<ClassicStageInfo>().sceneDirectorInteractibleCredits *= 20;
            Log.Info("Applied mad director " + SceneInfo.instance.GetComponent<ClassicStageInfo>().sceneDirectorInteractibleCredits );
            orig(self);
        };
    }
}