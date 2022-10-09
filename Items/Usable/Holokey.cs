using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class Holokey : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holokey");
            Tooltip.SetDefault("'Unlocks Holochests found in a crashed spaceship'"
                + "\nOnly one is needed");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
            SacrificeTotal = 1;
        }


        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 24;
            Item.rare = ItemRarityID.Cyan;
            Item.maxStack = 1;
        }
    }
}