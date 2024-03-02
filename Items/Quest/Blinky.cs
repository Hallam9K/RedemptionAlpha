using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Quest
{
    public class Blinky : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 2;
            ItemID.Sets.CanBePlacedOnWeaponRacks[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.DefaultToQuestFish();
        }
        public override bool IsQuestFish() => true;
        public override bool IsAnglerQuestAvailable() => Main.hardMode && RedeBossDowned.nukeDropped;

        public override void AnglerQuestChat(ref string description, ref string catchLocation)
        {
            description = Language.GetTextValue("Mods.Redemption.Dialogue.Angler.WastelandFish");
            catchLocation = Language.GetTextValue("Mods.Redemption.Dialogue.Angler.WastelandLocation");
        }
    }
}