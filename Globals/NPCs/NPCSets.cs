using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Globals.NPCs;

[ReinitializeDuringResizeArrays]
internal class NPCSets
{
    /// <summary> This NPC has Guard Points. </summary>
    public static bool[] UsesGuardPoints { get; } = NPCID.Sets.Factory.CreateNamedSet(nameof(UsesGuardPoints)).Description("This NPC uses Guard Points.")
        .RegisterBoolSet(defaultState: false, NPCID.GreekSkeleton, NPCID.AngryBonesBig, NPCID.AngryBonesBigHelmet, NPCID.AngryBonesBigMuscle, NPCID.GoblinWarrior, NPCID.ArmoredSkeleton, NPCID.ArmoredViking, NPCID.PossessedArmor, NPCID.BlueArmoredBones, NPCID.BlueArmoredBonesMace, NPCID.BlueArmoredBonesNoPants, NPCID.BlueArmoredBonesSword, NPCID.RustyArmoredBonesAxe, NPCID.RustyArmoredBonesFlail, NPCID.RustyArmoredBonesSword, NPCID.HellArmoredBones, NPCID.HellArmoredBonesMace, NPCID.HellArmoredBonesSpikeShield, NPCID.HellArmoredBonesSword, NPCID.Paladin);
}
