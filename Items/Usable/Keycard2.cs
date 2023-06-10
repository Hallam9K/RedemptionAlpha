using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Items.Usable
{
    public class Keycard2 : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Special Keycard");
            // Tooltip.SetDefault("Unlocks a special Laboratory Chest");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 34;
            Item.rare = ItemRarityID.Purple;
            Item.maxStack = 1;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
        }
    }
}