using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Plants
{
    public class IrradiatedCrimsonGrassSeeds : ModItem
	{
		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Irradiated Crimson Seeds");
            Tooltip.SetDefault("Plants irradiated crimson grass on dirt");
            SacrificeTotal = 25;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
			Item.height = 18;
			Item.maxStack = 9999;
			Item.rare = ItemRarityID.Green;
			Item.value = 10;

			Item.useStyle = ItemUseStyleID.Swing;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.autoReuse = true;
			Item.useTurn = true;
			Item.createTile = ModContent.TileType<IrradiatedCrimsonGrassTile>();
			Item.consumable = true;		
        }

		public override bool CanUseItem(Player p)
		{
			Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
			if(tile != null && tile.HasTile && tile.TileType == ModContent.TileType<IrradiatedDirtTile>())
			{
				WorldGen.destroyObject = true;
				TileID.Sets.BreakableWhenPlacing[ModContent.TileType<IrradiatedDirtTile>()] = true;
				return base.CanUseItem(p);
			}
			return false;
		}

		public override bool? UseItem(Player p)
		{
			WorldGen.destroyObject = false;
			TileID.Sets.BreakableWhenPlacing[ModContent.TileType<IrradiatedDirtTile>()] = false;
			return base.UseItem(p);
		}
	}
}