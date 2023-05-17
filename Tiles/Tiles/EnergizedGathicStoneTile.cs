using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts.Tiles;
using Redemption.Globals;
using Redemption.Items.Placeable.Tiles;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class EnergizedGathicStoneTile : ModTile
    {
        private Asset<Texture2D> glowTexture;
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<AncientHallBrickTile>()] = true;
            Main.tileMerge[ModContent.TileType<AncientHallBrickTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicGladestoneBrickTile>()] = true;
            Main.tileMerge[ModContent.TileType<GathicGladestoneBrickTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<AncientDirtTile>()] = true;
            Main.tileMerge[ModContent.TileType<AncientDirtTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<AncientGrassTile>()] = true;
            Main.tileMerge[ModContent.TileType<AncientGrassTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicStoneBrickTile>()] = true;
            Main.tileMerge[ModContent.TileType<GathicStoneBrickTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicGladestoneTile>()] = true;
            Main.tileMerge[ModContent.TileType<GathicGladestoneTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicStoneTile>()] = true;
            Main.tileMerge[ModContent.TileType<GathicStoneTile>()][Type] = true;
            RegisterItemDrop(ModContent.ItemType<GathicStone>());
            DustType = ModContent.DustType<SlateDust>();
            HitSound = CustomSounds.StoneHit;
            MinPick = 0;
            MineResist = 5f;
            AddMapEntry(new Color(206, 130, 68));
            if (!Main.dedServ)
                glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow");
        }
        private float drawTimer;
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;

            int height = tile.TileFrameY == 36 ? 18 : 16;

            RedeDraw.DrawTreasureBagEffect(spriteBatch, glowTexture.Value, ref drawTimer, new Vector2((i * 16) - (int)Main.screenPosition.X, (j * 16) - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), RedeColor.FadeColour1, 0, Vector2.Zero, 1f, 0);

            Main.spriteBatch.Draw(glowTexture.Value, new Vector2((i * 16) - (int)Main.screenPosition.X, (j * 16) - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), RedeColor.COLOR_GLOWPULSE, 0f, Vector2.Zero, 1f, 0, 0f);
        }
        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (Main.rand.NextBool(50))
                Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, DustID.Sandnado);
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.6f;
            g = 0.5f;
            b = 0.4f;
        }
        public override bool CanExplode(int i, int j) => false;
    }
}