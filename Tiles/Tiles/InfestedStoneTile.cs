using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.NPCs.Critters;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class InfestedStoneTile : ModTile
    {
        public override string Texture => "Terraria/Images/Tiles_" + TileID.Stone;
        private Asset<Texture2D> OverlayTexture;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileStone[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileMerge[Type][TileID.Stone] = true;
            RegisterItemDrop(ItemID.StoneBlock, 0);
            DustType = DustID.Stone;
            HitSound = SoundID.Tink;
            MinPick = 0;
            MineResist = 1.5f;
            AddMapEntry(new Color(128, 128, 128));
            if (!Main.dedServ)
                OverlayTexture = Request<Texture2D>("Redemption/Tiles/Tiles/InfestedStoneTile_Overlay");
        }
        public override bool IsTileDangerous(int i, int j, Player player) => true;

        public bool breakCheck;
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!fail && !WorldGen.gen)
            {
                if (Main.rand.NextBool(2) && Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.NewNPC(new EntitySource_TileBreak(i, j), i * 16 + 8, j * 16, ModContent.NPCType<SpiderSwarmer>());

                if (breakCheck)
                    return;
                breakCheck = true;
                for (int k = i - 3; k <= i + 3; k++)
                {
                    for (int l = j - 3; l <= j + 3; l++)
                    {
                        if ((k != i || l != j) && Main.tile[k, l].HasTile && Main.tile[k, l].TileType == ModContent.TileType<InfestedStoneTile>() && !Main.rand.NextBool(3))
                        {
                            WorldGen.KillTile(k, l, noItem: true);
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, k, l);
                        }
                    }
                }
                breakCheck = false;
            }
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (!TileDrawing.IsVisible(tile))
                return;
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;

            int height = tile.TileFrameY == 36 ? 18 : 16;
            Color color = Lighting.GetColor(i, j);
            Main.spriteBatch.Draw(OverlayTexture.Value, new Vector2((i * 16) - (int)Main.screenPosition.X, (j * 16) - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), color, 0f, Vector2.Zero, 1f, 0, 0f);
        }
    }
}