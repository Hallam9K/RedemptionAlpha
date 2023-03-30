using Redemption.Tiles.Furniture.Shade;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Shade
{
    public class ShadestonePillar1 : ModItem
    {
        public override string Texture => "Redemption/Items/Placeable/Furniture/Shade/ShadestonePillar";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadestone Pillar");
            /* Tooltip.SetDefault("4x20" +
                "\n[c/ff0000:Unbreakable (500% Pickaxe Power)]"); */
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ShadestonePillar1Tile>(), 0);
            Item.width = 30;
            Item.height = 48;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;
        }
    }
    public class ShadestonePillar2 : ModItem
    {
        public override string Texture => "Redemption/Items/Placeable/Furniture/Shade/ShadestonePillar";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadestone Pillar");
            /* Tooltip.SetDefault("4x10" +
                "\n[c/ff0000:Unbreakable (500% Pickaxe Power)]"); */
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ShadestonePillar2Tile>(), 0);
            Item.width = 30;
            Item.height = 48;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;
        }
    }
}