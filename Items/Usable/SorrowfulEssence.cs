using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class SorrowfulEssence : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Having this in your inventory may attract the Keeper's first creation underground" +
                "\n[i:" + ModContent.ItemType<BadRoute>() + "]");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 11));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemIconPulse[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;

            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 34;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.Blue;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.Purple.ToVector3() * 0.6f * Main.essScale);
        }
    }
}
