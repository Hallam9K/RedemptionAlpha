using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Items.Usable.Summons;
using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Misc
{
    public class GolemEyeTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            HitSound = CustomSounds.CrystalHit;
            DustType = DustID.Sandnado;
            MinPick = 0;
            MineResist = 2f;
            AddMapEntry(new Color(241, 215, 108));
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.9f;
            g = 0.7f;
            b = 0.5f;
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (Main.rand.NextBool(6))
                TileLoader.RandomUpdate(i, j, Type);

            int Score = 0;
            for (int w = -1; w < 2; w++)
            {
                for (int h = -1; h < 2; h++)
                {
                    int type = Framing.GetTileSafely(i + w, j + h).TileType;
                    if (type == ModContent.TileType<EnergizedGathicStoneTile>())
                        Score++;
                }
            }
            if (Framing.GetTileSafely(i, j + 2).TileType == ModContent.TileType<EnergizedGathicStoneTile>())
                Score++;
            if (Framing.GetTileSafely(i, j - 2).TileType == ModContent.TileType<EnergizedGathicStoneTile>())
                Score++;

            if (Score >= 10)
            {
                Main.StartRain();
                Main.SyncRain();
                Main.NewLightning();
                RedeDraw.SpawnExplosion(new Vector2(i * 16 + 8, j * 16 + 8), Color.White, noDust: true, tex: Redemption.HolyGlow3.Value);
                SoundEngine.PlaySound(SoundID.Item68, new Vector2(i * 16, j * 16));
                SoundEngine.PlaySound(CustomSounds.Thunderstrike, new Vector2(i * 16, j * 16));
                Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = new Vector2(i * 16, j * 16);
                Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 20;
                Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ModContent.ItemType<AncientSigil>());
                for (int w = -2; w < 3; w++)
                {
                    for (int h = -2; h < 3; h++)
                    {
                        int type = Framing.GetTileSafely(i + w, j + h).TileType;
                        if (type == ModContent.TileType<EnergizedGathicStoneTile>() || type == ModContent.TileType<GolemEyeTile>())
                            WorldGen.KillTile(i + w, j + h, noItem: true);
                    }
                }
            }
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (!NPC.downedMoonlord)
                return true;

            Texture2D flare = Redemption.WhiteFlare.Value;
            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;
            Vector2 origin = new(flare.Width / 2f, flare.Height / 2f);
            Color color = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, new Color(241, 215, 108), new Color(255, 255, 255), new Color(241, 215, 108));

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);

            spriteBatch.Draw(flare, new Vector2(i * 16 + 8 - (int)Main.screenPosition.X, j * 16 + 8 - (int)Main.screenPosition.Y) + zero, new Rectangle?(rect), color, Main.GlobalTimeWrappedHourly, origin, 1, 0, 1f);
            spriteBatch.Draw(flare, new Vector2(i * 16 + 8 - (int)Main.screenPosition.X, j * 16 + 8 - (int)Main.screenPosition.Y) + zero, new Rectangle?(rect), color, -Main.GlobalTimeWrappedHourly, origin, 1, 0, 1f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);
            return true;
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (!NPC.downedMoonlord)
                return;

            Texture2D hint = ModContent.Request<Texture2D>("Redemption/Textures/GolemEyeHint").Value;
            Rectangle rect2 = new(0, 0, hint.Width, hint.Height);
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;
            Vector2 origin2 = new(hint.Width / 2f, hint.Height / 2f);
            Color color = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, new Color(241, 215, 108), new Color(255, 255, 255), new Color(241, 215, 108));
            float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 0.7f, 0.1f, 0.7f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);

            spriteBatch.Draw(hint, new Vector2(i * 16 + 8 - (int)Main.screenPosition.X, j * 16 + 8 - (int)Main.screenPosition.Y) + zero, new Rectangle?(rect2), color * scale, 0, origin2, 1, 0, 1f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);
        }
        public override void RandomUpdate(int i, int j)
        {
            if (NPC.downedMoonlord)
            {
                WorldGen.SpreadGrass(i + Main.rand.Next(-2, 3), j + Main.rand.Next(-2, 3), ModContent.TileType<GathicStoneTile>(), ModContent.TileType<EnergizedGathicStoneTile>(), false);
                WorldGen.SpreadGrass(i + Main.rand.Next(-2, 3), j + Main.rand.Next(-2, 3), ModContent.TileType<GathicGladestoneTile>(), ModContent.TileType<EnergizedGathicStoneTile>(), false);
            }
        }
        public override bool CanExplode(int i, int j) => false;
    }
}