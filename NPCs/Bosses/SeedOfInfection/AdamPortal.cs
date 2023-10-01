using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.NPCs.Friendly;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.SeedOfInfection
{
    public class AdamPortal : ModProjectile
    {
        public override string Texture => "Redemption/Textures/PortalTex";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Portal to Another World (no way...)");
        }
        public override void SetDefaults()
        {
            Projectile.width = 188;
            Projectile.height = 188;
            Projectile.alpha = 255;
        }
        private float RotTime;
        public override void AI()
        {
            Player player = Main.player[(int)Projectile.ai[0]];
            Projectile.rotation += .02f;
            if (Projectile.localAI[1] > 0)
                player.RedemptionScreen().cutscene = true;
            switch (Projectile.localAI[1])
            {
                case 0:
                    Projectile.Center = player.Center + new Vector2(200, -200);
                    if (player.active && !player.dead && Projectile.localAI[0]++ >= 300 && !RedeHelper.BossActive() && player.ZoneOverworldHeight)
                    {
                        bool no = false;
                        for (int x = -3; x <= 3; x++)
                        {
                            for (int y = -3; y <= 3; y++)
                            {
                                Point tileToProj = Projectile.Center.ToTileCoordinates();
                                Tile tile = Framing.GetTileSafely(tileToProj.X + x, tileToProj.Y + y);
                                if (tile.HasTile)
                                    no = true;
                            }
                        }
                        if (!no)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, Projectile.position);
                            Projectile.localAI[0] = 0;
                            Projectile.localAI[1] = 1;
                            Projectile.netUpdate = true;
                        }
                    }
                    break;
                case 1:
                    if (Projectile.localAI[0]++ == 0)
                        Projectile.scale = 0.01f;
                    Projectile.alpha -= 4;
                    Projectile.scale += 0.02f;
                    if (Projectile.scale >= 1)
                    {
                        Projectile.scale = 1;
                        Projectile.localAI[0] = 0;
                        Projectile.localAI[1] = 2;
                        Projectile.netUpdate = true;
                    }
                    break;
                case 2:
                    if (Projectile.localAI[0]++ == 60)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy, Projectile.position);
                        RedeHelper.SpawnNPC(Projectile.GetSource_FromThis(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<TBot_Intro>());
                    }
                    if (Projectile.localAI[0] >= 120)
                    {
                        Projectile.localAI[0] = 0;
                        Projectile.localAI[1] = 3;
                        Projectile.netUpdate = true;
                    }
                    break;
                case 3:
                    Projectile.alpha += 4;
                    Projectile.scale -= 0.02f;
                    if (Projectile.scale <= 0.01f)
                        Projectile.Kill();
                    break;
            }
            if (Projectile.alpha <= 100)
            {
                if (Vector2.Distance(Main.screenPosition + new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), Projectile.Center) <= Main.screenWidth / 2 + 100)
                {
                    RotTime += (float)Math.PI / 120;
                    RotTime *= 1.01f;
                    if (RotTime >= Math.PI) RotTime = 0;
                    float timer = RotTime;
                    Terraria.Graphics.Effects.Filters.Scene.Activate("MoR:Shockwave", Projectile.Center)?.GetShader().UseProgress(timer).UseOpacity(100f * (1 - timer / 1.3f)).UseColor(2, 8, 5).UseTargetPosition(Projectile.Center);

                    if (RotTime > 0.5 && RotTime < 0.6 && !Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.PortalWub, Projectile.position);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.NegativeDye);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive(true);
            GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.LightGreen) * 0.6f, -Projectile.rotation, drawOrigin, Projectile.scale * 1.7f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.LightGreen), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();

            Texture2D extra = ModContent.Request<Texture2D>("Redemption/Textures/SpiritPortalTex").Value;
            Main.EntitySpriteDraw(extra, Projectile.Center - Main.screenPosition, null, Color.LightGreen * Projectile.Opacity * .4f, -Projectile.rotation * 1.5f, new Vector2(extra.Width / 2, extra.Height / 2), Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(extra, Projectile.Center - Main.screenPosition, null, Color.LightGreen * Projectile.Opacity * .2f, -Projectile.rotation * 2f, new Vector2(extra.Width / 2, extra.Height / 2), Projectile.scale * .7f, 0, 0);
            Main.EntitySpriteDraw(extra, Projectile.Center - Main.screenPosition, null, Color.LightGreen * Projectile.Opacity * .1f, -Projectile.rotation * 2.5f, new Vector2(extra.Width / 2, extra.Height / 2), Projectile.scale * .5f, 0, 0);
            return false;
        }
    }
}