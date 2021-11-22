using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Placeable.Plants;
using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Plants
{
    public class RadRootTile : ModTile
	{
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.SwaysInWindBasic[Type] = true;
            DustType = DustID.GreenBlood;
            SoundType = SoundID.Grass;
            Main.tileLighted[Type] = true;
            Main.tileSpelunker[Type] = true;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Rad Root");
            AddMapEntry(Color.DarkOliveGreen, name);
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = new int[] { 20 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.AlternateTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.AnchorValidTiles = new int[]
            {
                ModContent.TileType<IrradiatedCorruptGrassTile>(),
				ModContent.TileType<IrradiatedCrimsonGrassTile>(),
				ModContent.TileType<IrradiatedEbonstoneTile>(),
				ModContent.TileType<IrradiatedCrimstoneTile>(),
			};
            TileObjectData.newTile.AnchorAlternateTiles = new int[]
            {
                TileID.ClayPot,
                TileID.PlanterBox
            };
            TileObjectData.addTile(Type);
		}

		public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
		{
			if (i % 2 == 1)
			{
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
		}

		public override bool Drop(int i, int j)
		{
			int stage = Main.tile[i, j].frameX / 18;
            if (stage == 2)
				Item.NewItem(i * 16, j * 16, 0, 0, ModContent.ItemType<RadRoot>());
			return false;
		}

		public override void RandomUpdate(int i, int j)
		{
			if (Main.tile[i, j].frameX == 0)
				Main.tile[i, j].frameX += 18;
			else if (Main.tile[i, j].frameX == 18)
				Main.tile[i, j].frameX += 18;
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)   //light colors
        {
            int stage = Main.tile[i, j].frameX / 18;
            if (stage == 2)
            {
                r = 0.0f;
                g = 0.2f;
                b = 0.0f;
            }
            if (stage < 2)
            {
                r = 0;
                g = 0;
                b = 0;
            }
        }

    }
}
