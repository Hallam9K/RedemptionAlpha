using Redemption.NPCs.Minibosses.FowlEmperor;
using Terraria.ModLoader;

namespace Redemption.Achievements;

public class FowlEmperorKilled : ModAchievement
{
    public override string TextureName => "Redemption/Achievements/Achievements";
    public override int Index => 0;
    public override void SetStaticDefaults()
    {
        AddNPCKilledCondition(NPCType<FowlEmperor>());
    }
    public override Position GetDefaultPosition() => new Before("SLIPPERY_SHINOBI");
}