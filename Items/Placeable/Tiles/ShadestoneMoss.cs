using Redemption.Rarities;
using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class ShadestoneMoss : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Black Moss");
            // Tooltip.SetDefault("Plants moss on Shadestone and Shadestone Bricks");
            ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true;
            Item.ResearchUnlockCount = 25;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ShadestoneMossyTile>(), 0);
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.width = 20;
            Item.height = 18;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<SoullessRarity>();
        }
        public override bool CanUseItem(Player p)
        {
            Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
            if (tile != null && tile.HasTile && tile.TileType == ModContent.TileType<ShadestoneTile>())
            {
                Item.createTile = ModContent.TileType<ShadestoneMossyTile>();
                WorldGen.destroyObject = true;
                TileID.Sets.BreakableWhenPlacing[ModContent.TileType<ShadestoneTile>()] = true;
                return base.CanUseItem(p);
            }
            if (tile != null && tile.HasTile && tile.TileType == ModContent.TileType<ShadestoneBrickTile>())
            {
                Item.createTile = ModContent.TileType<ShadestoneBrickMossyTile>();
                WorldGen.destroyObject = true;
                TileID.Sets.BreakableWhenPlacing[ModContent.TileType<ShadestoneBrickTile>()] = true;
                return base.CanUseItem(p);
            }
            return false;
        }

        public override bool? UseItem(Player p)
        {
            WorldGen.destroyObject = false;
            TileID.Sets.BreakableWhenPlacing[ModContent.TileType<ShadestoneTile>()] = false;
            TileID.Sets.BreakableWhenPlacing[ModContent.TileType<ShadestoneBrickTile>()] = false;
            return base.UseItem(p);
        }
    }
}