using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Biomes;
using Redemption.Dusts.Tiles;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Misc
{
    public class WastelandTorchTile : ModTile
	{
		private Asset<Texture2D> flameTexture;

		public override void SetStaticDefaults()
		{
			// Properties
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileSolid[Type] = false;
			Main.tileNoAttach[Type] = true;
			Main.tileNoFail[Type] = true;
			Main.tileWaterDeath[Type] = true;
			TileID.Sets.FramesOnKillWall[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.Torch[Type] = true;

			DustType = ModContent.DustType<WastelandTorchDust>();
			AdjTiles = new int[] { TileID.Torches };

			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.Torches, 0));
            TileObjectData.addTile(Type);

			AddMapEntry(new Color(255, 199, 199), Language.GetText("ItemName.Torch"));

			// Assets
			if (!Main.dedServ)
			{
				flameTexture = ModContent.Request<Texture2D>(Texture + "_Flame");
			}
		}
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;

            // We can determine the item to show on the cursor by getting the tile style and looking up the corresponding item drop.
            int style = TileObjectData.GetTileStyle(Main.tile[i, j]);
            player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type, style);
        }
        public override float GetTorchLuck(Player player)
		{
			bool inWastelandBiome = Main.LocalPlayer.InModBiome<WastelandPurityBiome>();
			return inWastelandBiome ? 1f : 0;
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = Main.rand.Next(1, 3);

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			if (tile.TileFrameX < 66)
			{
				r = 0.85f;
				g = 0.7f;
				b = 0.7f;
			}
		}

		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
		{
			offsetY = 0;
			if (WorldGen.SolidTile(i, j - 1))
                offsetY = 4;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			int offsetY = 0;
			if (WorldGen.SolidTile(i, j - 1))
                offsetY = 4; 
			Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
			if (Main.drawToScreen)
				zero = Vector2.Zero;

			ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (uint)i);
			Color color = new(100, 100, 100, 0);
			int width = 20;
			int height = 20;
			var tile = Main.tile[i, j];
			int frameX = tile.TileFrameX;
			int frameY = tile.TileFrameY;
            for (int k = 0; k < 7; k++)
			{
				float xx = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
				float yy = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;

				spriteBatch.Draw(flameTexture.Value, new Vector2(i * 16 - (int)Main.screenPosition.X - (width - 16f) / 2f + xx, j * 16 - (int)Main.screenPosition.Y + offsetY + yy) + zero, new Rectangle(frameX, frameY, width, height), color, 0f, default, 1f, SpriteEffects.None, 0f);
			}
		}
	}
}