using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Usable;
using Redemption.NPCs.Friendly;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Redemption.Globals.RedeNet;

namespace Redemption.Tiles.Natural
{
    public class SkeletonRemainsTile3 : ModTile
    {
        public override string Texture => "Redemption/Tiles/Natural/SkeletonRemainsTile1";
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            DustType = DustID.DungeonSpirit;
            MinPick = 500;
            MineResist = 50;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Skeletal Remains");
            AddMapEntry(new Color(229, 229, 195));
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.3f;
            g = 0.3f;
            b = 0.6f;
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (Main.tile[i, j].TileFrameX == 0 && Main.tile[i, j].TileFrameY == 0)
            {
                if (Main.rand.NextBool(20))
                {
                    int d = Dust.NewDust(new Vector2(i * 16 + 12, j * 16 + 16), 54, 14, DustID.DungeonSpirit);
                    Main.dust[d].velocity.Y -= 3f;
                    Main.dust[d].noGravity = true;
                }
            }
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;

            int height = tile.TileFrameY == 36 ? 18 : 16;
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Redemption/Tiles/Natural/SkeletonRemainsTile1_Glow").Value, new Vector2((i * 16) - (int)Main.screenPosition.X, (j * 16) - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
        public override bool CanExplode(int i, int j) => false;
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            if (!player.RedemptionAbility().Spiritwalker)
            {
                player.cursorItemIconEnabled = true;
                player.cursorItemIconID = ModContent.ItemType<DeadRinger>();
            }
            else
            {
                player.cursorItemIconEnabled = false;
                player.cursorItemIconID = 0;
            }
        }
        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            if (player.HeldItem.type == ModContent.ItemType<DeadRinger>() && !player.RedemptionAbility().Spiritwalker)
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<SpiritwalkerSoul>()))
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int index1 = NPC.NewNPC(new EntitySource_TileInteraction(player, i, j), i * 16, (j + 1) * 16, ModContent.NPCType<SpiritwalkerSoul>());
                        SoundEngine.PlaySound(SoundID.Item74, Main.npc[index1].position);
                    }
                    else
                    {
                        if (Main.netMode == NetmodeID.SinglePlayer)
                            return false;

                        Redemption.WriteToPacket(Redemption.Instance.GetPacket(), (byte)ModMessageType.NPCSpawnFromClient, ModContent.NPCType<SpiritwalkerSoul>(), new Vector2(i * 16, (j + 1) * 16)).Send(-1);
                        SoundEngine.PlaySound(SoundID.Item74, player.position);
                    }
                    for (int n = 0; n < 25; n++)
                    {
                        int dustIndex = Dust.NewDust(new Vector2(i * 16, (j + 1) * 16), 2, 2, DustID.DungeonSpirit, 0f, 0f, 100, default, 2);
                        Main.dust[dustIndex].velocity *= 2f;
                        Main.dust[dustIndex].noGravity = true;
                    }
                    DustHelper.DrawDustImage(new Vector2(i * 16, (j + 1) * 16), DustID.DungeonSpirit, 0.5f, "Redemption/Effects/DustImages/DeadRingerDust", 2, true, 0);
                }
            }
            return true;
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (!Main.LocalPlayer.RedemptionAbility().SpiritwalkerActive)
                return true;

            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;
            Vector2 origin = new(flare.Width / 2f, flare.Height / 2f);
            if (Main.tile[i, j].TileFrameX == 0 && Main.tile[i, j].TileFrameY == 0)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);

                spriteBatch.Draw(flare, new Vector2(((i + 2.45f) * 16) - (int)Main.screenPosition.X, ((j + 1.7f) * 16) - (int)Main.screenPosition.Y) + zero, new Rectangle?(rect), RedeColor.COLOR_GLOWPULSE, Main.GlobalTimeWrappedHourly, origin, 1.5f, 0, 1f);
                spriteBatch.Draw(flare, new Vector2(((i + 2.45f) * 16) - (int)Main.screenPosition.X, ((j + 1.7f) * 16) - (int)Main.screenPosition.Y) + zero, new Rectangle?(rect), RedeColor.COLOR_GLOWPULSE, -Main.GlobalTimeWrappedHourly, origin, 1.5f, 0, 1f);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);
            }
            return true;
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            if (!WorldGen.gen && Main.netMode != NetmodeID.MultiplayerClient && !Main.LocalPlayer.RedemptionAbility().Spiritwalker)
                NPC.NewNPC(new EntitySource_TileBreak(i, j), i * 16 + 32, j * 16, ModContent.NPCType<SpiritwalkerSoul>());

            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 64, 32, ModContent.ItemType<GraveSteelShards>(), Main.rand.Next(5, 9));
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
    public class SkeletonRemains3 : PlaceholderTile
    {
        public override string Texture => "Redemption/Placeholder";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skeletal Remains (Soulful)");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<SkeletonRemainsTile3>();
        }
    }
}