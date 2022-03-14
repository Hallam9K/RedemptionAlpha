using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Base;

namespace Redemption.Items.Usable
{
    public class Keycard2 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Special Keycard");
            Tooltip.SetDefault("Unlocks a special Laboratory Chest");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 30;
            Item.rare = ItemRarityID.Purple;
            Item.maxStack = 1;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Item.ModItem.Texture + "_Glow").Value;
        }
    }
}