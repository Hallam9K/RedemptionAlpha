using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Plants
{
    public class AncientGrassSeeds : ModItem
	{
		public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Plants grass on ancient dirt");
            ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true;
            Item.ResearchUnlockCount = 25;
        }
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Green;
            Item.value = 10;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.createTile = ModContent.TileType<AncientGrassTile>();
            Item.consumable = true;
        }
        public override bool CanUseItem(Player p)
        {
            Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
            if (tile != null && tile.HasTile && tile.TileType == ModContent.TileType<AncientDirtTile>())
            {
                WorldGen.destroyObject = true;
                TileID.Sets.BreakableWhenPlacing[ModContent.TileType<AncientDirtTile>()] = true;
                return base.CanUseItem(p);
            }
            return false;
        }
        public override bool? UseItem(Player p)
        {
            WorldGen.destroyObject = false;
            TileID.Sets.BreakableWhenPlacing[ModContent.TileType<AncientDirtTile>()] = false;
            return base.UseItem(p);
        }
    }
}