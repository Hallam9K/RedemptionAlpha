using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Items.Accessories.PostML;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Misc
{
    public class CelestialStar : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Star");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 400;
            Projectile.alpha = 50;
            Projectile.scale = 0.01f;
            Projectile.frame = Main.rand.Next(3);
            Projectile.rotation = RedeHelper.RandomRotation();
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            int npc = (int)Projectile.ai[0];
            if (npc < 0 || npc >= 200 || !Main.npc[npc].active)
            {
                Projectile.timeLeft = 400;
                Projectile.localAI[0] = 1;
            }
            Projectile.scale += 0.1f;
            Projectile.scale = MathHelper.Min(Projectile.scale, 1);
            switch (Projectile.ai[1])
            {
                default:
                    if (Projectile.localAI[0] == 1 || player.GetModPlayer<WaterfowlEgg_Player>().equipped)
                    {
                        Projectile.Move(player.Center, Projectile.timeLeft > 100 ? 30 : 60, 10);
                        Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

                        if (Projectile.Hitbox.Intersects(player.Hitbox))
                        {
                            SoundEngine.PlaySound(SoundID.MaxMana with { Volume = 0.5f }, player.position);
                            player.statMana++;
                            player.Heal(2);
                            player.ManaEffect(1);
                            Projectile.Kill();
                        }
                    }
                    break;
                // Solar armour effect
                case 1:
                    Projectile.GetGlobalProjectile<ElementalProjectile>().OverrideElement[ElementID.Fire] = ElementID.AddElement;
                    Projectile.Move(Main.npc[npc].Center, 2, 30);
                    if (Projectile.localAI[1]++ > 120)
                    {
                        RedeHelper.NPCRadiusDamage(60, Projectile, player.statDefense * 2, 0);
                        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot with { Volume = .4f, Pitch = 0.1f }, Projectile.position);
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<WickerManEmberExplosion_Visual>(), 0, 0, player.whoAmI, -2);
                        Projectile.Kill();
                    }
                    break;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[1] is 1)
                target.AddBuff(BuffID.OnFire3, 120);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 4;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, height / 2);
            float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1.1f, 1f, 1.1f);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            switch (Projectile.ai[1])
            {
                default:
                    for (int k = 0; k < Projectile.oldPos.Length; k++)
                    {
                        Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin - new Vector2(6, 6);
                        Color color = Color.White * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                        Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(rect), color, Projectile.rotation, drawOrigin, Projectile.scale * scale, 0, 0);
                    }

                    Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Color.White, Projectile.rotation, drawOrigin, Projectile.scale * scale, 0, 0);
                    break;
                case 1:
                    for (int k = 0; k < Projectile.oldPos.Length; k++)
                    {
                        Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin - new Vector2(6, 6) + RedeHelper.Spread(4);
                        Color color = Color.Orange * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                        Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(rect), color, Projectile.rotation, drawOrigin, Projectile.scale * scale, 0, 0);
                    }
                    Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Color.Orange, Projectile.rotation, drawOrigin, Projectile.scale * scale, 0, 0);
                    break;
            }

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SilverCoin, Scale: 2f);
            Main.dust[dustIndex].velocity *= 0f;
        }
    }
    public class WickerManEmberExplosion_Visual : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            ElementID.ProjFire[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
        public override void OnSpawn(IEntitySource source)
        {
        }
        private float GlowTimer;
        private bool Glow;
        private float Rot;
        public Color color = Color.Orange;
        public float dustScale = 1f;
        public float scale = 3f;
        public Texture2D texture;
        public override void AI()
        {
            if (Glow)
            {
                GlowTimer += 3;
                if (GlowTimer > 60)
                {
                    Glow = false;
                    GlowTimer = 0;
                }
            }
            if (Projectile.localAI[0]++ == 0)
            {
                Glow = true;
                Projectile.alpha = 255;
                Rot = Main.rand.NextFloat(MathHelper.PiOver4, 3 * MathHelper.PiOver4);

                float shake = Projectile.ai[0] == -2 ? 0 : 1;
                Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
                Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += shake;
                for (int i = 0; i < 15; i++)
                {
                    int dust = Dust.NewDust(Projectile.Center, 1, 1, ModContent.DustType<GlowDust>(), Scale: 1);
                    Main.dust[dust].velocity *= 3 + Projectile.ai[0];
                    Main.dust[dust].noGravity = true;
                    Color dustColor = new(color.R, color.G, color.B) { A = 0 };
                    Main.dust[dust].color = dustColor;
                }
            }
            if (Projectile.localAI[0] >= 20)
                Projectile.Kill();
        }
        public override void PostDraw(Color lightColor)
        {
            texture = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Texture2D teleportGlow = texture;
            Rectangle rect2 = new(0, 0, teleportGlow.Width, teleportGlow.Height);
            Vector2 origin2 = new(teleportGlow.Width / 2, teleportGlow.Height / 2);
            Vector2 position2 = Projectile.Center - Main.screenPosition;
            Color colour2 = Color.Lerp(color, color, 1f / GlowTimer * 10f) * (1f / GlowTimer * 10f);
            if (Glow)
            {
                Main.spriteBatch.Draw(teleportGlow, position2, new Rectangle?(rect2), colour2, Rot, origin2, scale, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(teleportGlow, position2, new Rectangle?(rect2), colour2 * 0.4f, Rot, origin2, scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
        }
    }
}