using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Biomes;
using Redemption.Dusts;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Usable;
using Redemption.NPCs.Critters;
using Redemption.Textures;
using Redemption.Tiles.Natural;
using Redemption.Tiles.Plants;
using Redemption.Tiles.Tiles;
using Redemption.WorldGeneration;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public class RedeGlobalTile : GlobalTile
    {
        public override void NearbyEffects(int i, int j, int type, bool closer)
        {
            Tile topperTile = Framing.GetTileSafely(i, --j);

            if (closer && (Main.LocalPlayer.InModBiome<WastelandPurityBiome>() || Main.LocalPlayer.InModBiome<LabBiome>()) &&
                topperTile.LiquidAmount > 0 && topperTile.LiquidType == LiquidID.Water)
            {
                for (; j > 0 && Main.tile[i, j - 1] != null && Main.tile[i, j - 1].LiquidAmount > 0 && Main.tile[i, j - 1].LiquidType == LiquidID.Water; --j);

                if (Main.rand.NextBool(200))
                    Dust.NewDust(new Vector2(i * 16, j * 16), 0, 0, ModContent.DustType<XenoWaterDust>(), Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-4f, -2f));
            }
        }
        public override bool CanDrop(int i, int j, int type)
        {
            if (type == ModContent.TileType<IrradiatedDirtTile>() && TileID.Sets.BreakableWhenPlacing[ModContent.TileType<IrradiatedDirtTile>()])
                return false;
            if (type == ModContent.TileType<AncientDirtTile>() && TileID.Sets.BreakableWhenPlacing[ModContent.TileType<AncientDirtTile>()])
                return false;
            if (type == ModContent.TileType<ShadestoneBrickTile>() && TileID.Sets.BreakableWhenPlacing[ModContent.TileType<ShadestoneBrickTile>()])
                return false;
            if (type == ModContent.TileType<ShadestoneTile>() && TileID.Sets.BreakableWhenPlacing[ModContent.TileType<ShadestoneTile>()])
                return false;
            return base.CanDrop(i, j, type);
        }
        public override void Drop(int i, int j, int type)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && !WorldGen.noTileActions && !WorldGen.gen)
            {
                if (type == TileID.Trees && Main.tile[i, j + 1].TileType == TileID.Grass)
                {
                    if (Main.rand.NextBool(6))
                        Projectile.NewProjectile(new EntitySource_TileBreak(i, j), i * 16, (j - 10) * 16, -4 + Main.rand.Next(0, 7), -3 + Main.rand.Next(-3, 0), ModContent.ProjectileType<TreeBugFall>(), 0, 0);
                }
                if (type == TileID.PalmTree && Main.tile[i, j + 1].TileType == TileID.Sand)
                {
                    if (Main.rand.NextBool(6))
                        Projectile.NewProjectile(new EntitySource_TileBreak(i, j), i * 16, (j - 10) * 16, -4 + Main.rand.Next(0, 7), -3 + Main.rand.Next(-3, 0), ModContent.ProjectileType<CoastScarabFall>(), 0, 0);
                }
            }
            if ((type == TileID.LeafBlock || type == TileID.LivingMahoganyLeaves) && Main.rand.NextBool(4))
                Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ModContent.ItemType<LivingTwig>());
        }
        public override void RandomUpdate(int i, int j, int type)
        {
            if (type == TileID.Grass && !Main.dayTime && RedeBossDowned.downedThorn)
            {
                if (!Framing.GetTileSafely(i, j - 1).HasTile && Main.tile[i, j].HasTile && Main.tile[i, j - 1].LiquidAmount == 0 && Main.tile[i, j - 1].WallType == 0)
                {
                    if (Main.rand.NextBool(300))
                        WorldGen.PlaceTile(i, j - 1, ModContent.TileType<NightshadeTile>(), true);
                }
            }
            if (type == TileID.Grass && Main.dayTime && RedeBossDowned.downedThorn)
            {
                if (!Framing.GetTileSafely(i, j - 1).HasTile && !Framing.GetTileSafely(i + 1, j - 1).HasTile && Main.tile[i, j].HasTile && Main.tile[i + 1, j].HasTile && Main.tile[i, j - 1].LiquidAmount == 0 && Main.tile[i, j - 1].WallType == 0)
                {
                    if (Main.rand.NextBool(6000))
                        WorldGen.PlaceTile(i, j - 1, ModContent.TileType<AnglonicMysticBlossomTile>(), true);
                }
            }
            if (RedeGen.cryoCrystalSpawn && TileID.Sets.Conversion.Ice[type])
            {
                bool tileUp = !Framing.GetTileSafely(i, j - 1).HasTile;
                bool tileDown = !Framing.GetTileSafely(i, j + 1).HasTile;
                bool tileLeft = !Framing.GetTileSafely(i - 1, j).HasTile;
                bool tileRight = !Framing.GetTileSafely(i + 1, j).HasTile;
                if (Main.rand.NextBool(1200) && j > (int)(Main.maxTilesY * .25f))
                {
                    if (tileUp)
                    {
                        WorldGen.PlaceObject(i, j - 1, ModContent.TileType<CryoCrystalTile>(), true);
                        NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<CryoCrystalTile>(), 0, 0, -1, -1);
                    }
                    else if (tileDown)
                    {
                        WorldGen.PlaceObject(i, j + 1, ModContent.TileType<CryoCrystalTile>(), true);
                        NetMessage.SendObjectPlacement(-1, i, j + 1, ModContent.TileType<CryoCrystalTile>(), 0, 0, -1, -1);
                    }
                    else if (tileLeft)
                    {
                        WorldGen.PlaceObject(i - 1, j, ModContent.TileType<CryoCrystalTile>(), true);
                        NetMessage.SendObjectPlacement(-1, i - 1, j, ModContent.TileType<CryoCrystalTile>(), 0, 0, -1, -1);
                    }
                    else if (tileRight)
                    {
                        WorldGen.PlaceObject(i + 1, j, ModContent.TileType<CryoCrystalTile>(), true);
                        NetMessage.SendObjectPlacement(-1, i + 1, j, ModContent.TileType<CryoCrystalTile>(), 0, 0, -1, -1);
                    }
                }
            }
        }

        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
            if (Main.tile[i, j - 1].HasTile && RedeTileHelper.CannotMineTileBelow[Main.tile[i, j - 1].TileType])
                return false;
            if (Main.tile[i, j + 1].HasTile && RedeTileHelper.CannotMineTileAbove[Main.tile[i, j + 1].TileType])
                return false;
            return base.CanKillTile(i, j, type, ref blockDamaged);
        }

        public override bool CanExplode(int i, int j, int type)
        {
            if (Main.tile[i, j - 1].HasTile && RedeTileHelper.CannotMineTileBelow[Main.tile[i, j - 1].TileType])
                return false;
            if (Main.tile[i, j + 1].HasTile && RedeTileHelper.CannotMineTileAbove[Main.tile[i, j + 1].TileType])
                return false;
            return base.CanExplode(i, j, type);
        }

        public override bool Slope(int i, int j, int type)
        {
            if (Main.tile[i, j - 1].HasTile && RedeTileHelper.CannotMineTileBelow[Main.tile[i, j - 1].TileType])
                return false;
            if (Main.tile[i, j + 1].HasTile && RedeTileHelper.CannotMineTileAbove[Main.tile[i, j + 1].TileType])
                return false;
            return base.Slope(i, j, type);
        }
        public override void DrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            Tile tile = Main.tile[i, j];
            if (Main.netMode != NetmodeID.Server && ModContent.GetInstance<ThornScene>().IsSceneEffectActive(Main.LocalPlayer) && !Main.tile[i, j - 1].HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType] && Main.rand.NextBool(24) && ((Main.drawToScreen && Main.rand.NextBool(18)) || !Main.drawToScreen))
            {
                Vector2 val = Utils.ToWorldCoordinates(new Point(i, j), 8f, 8f);
                int goreType = Main.rand.Next(1202, 1205);
                float s = 1f;
                if (goreType is 1202)
                    s = 2f;
                float scale = s + Main.rand.NextFloat() * 1.6f;
                Vector2 position5 = val + new Vector2(0f, -18f);
                Vector2 velocity = Main.rand.NextVector2Circular(0.7f, 0.25f) * 0.4f + Main.rand.NextVector2CircularEdge(1f, 0.4f) * 0.1f;
                velocity *= 4f;
                velocity.Y = 0;
                Gore.NewGorePerfect(new EntitySource_TileUpdate(i, j), position5, velocity, goreType, scale);
            }
        }
    }
    public static class RedeTileHelper
    {
        public static bool[] CannotMineTileBelow = TileID.Sets.Factory.CreateBoolSet();
        public static bool[] CannotMineTileAbove = TileID.Sets.Factory.CreateBoolSet();
        public static bool[] CannotTeleportInFront = WallID.Sets.Factory.CreateBoolSet();
        public static bool CanDeadRing(Terraria.Player player)
        {
            return player.HeldItem.type == ModContent.ItemType<DeadRinger>() || (player.RedemptionAbility().SpiritwalkerActive && player.HasItemInAnyInventory(ModContent.ItemType<DeadRinger>()));
        }
        public static void DrawSpiritFlare(SpriteBatch spriteBatch, int i, int j, int frameX = 0, float xOffset = 0, float yOffset = 0, float scale = 1.5f)
        {
            if (!Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive)
                return;

            Asset<Texture2D> flare = CommonTextures.WhiteFlare;
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;
            Vector2 origin = flare.Size() / 2;

            bool frameCheck = Main.tile[i, j].TileFrameX == 0;
            if (frameX != 0)
                frameCheck = Main.tile[i, j].TileFrameX % frameX == 0;
            if (frameCheck && Main.tile[i, j].TileFrameY == 0)
            {
                spriteBatch.Draw(flare.Value, new Vector2(((i + xOffset) * 16) - (int)Main.screenPosition.X, ((j + yOffset) * 16) - (int)Main.screenPosition.Y) + zero, null, RedeColor.COLOR_GLOWPULSE with { A = 0 }, Main.GlobalTimeWrappedHourly, origin, scale, 0, 1f);
                spriteBatch.Draw(flare.Value, new Vector2(((i + xOffset) * 16) - (int)Main.screenPosition.X, ((j + yOffset) * 16) - (int)Main.screenPosition.Y) + zero, null, RedeColor.COLOR_GLOWPULSE with { A = 0 }, -Main.GlobalTimeWrappedHourly, origin, scale, 0, 1f);
            }
        }
        public static void SimpleGlowmask(int i, int j, Color color, string texture)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (!TileDrawing.IsVisible(tile))
                return;
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;

            int height = tile.TileFrameY == 36 ? 18 : 16;
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(texture + "_Glow").Value, new Vector2((i * 16) - (int)Main.screenPosition.X, (j * 16) - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
        public static void SimpleGlowmask(int i, int j, Color color, string tex, int animFrameHeight, int type, int offset = 0)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (!TileDrawing.IsVisible(tile))
                return;
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;

            int height = tile.TileFrameY % animFrameHeight >= 16 ? 18 : 16;
            int animate = Main.tileFrame[type] * animFrameHeight;

            Texture2D texture = ModContent.Request<Texture2D>(tex + "_Glow").Value;
            Rectangle frame = new(tile.TileFrameX, tile.TileFrameY + animate, 16, height);
            Main.spriteBatch.Draw(texture, new Vector2((i * 16) - (int)Main.screenPosition.X, (j * 16) - offset - (int)Main.screenPosition.Y) + zero, frame, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}