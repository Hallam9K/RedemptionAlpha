using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.ID;
using Redemption.Globals;
using Terraria.ModLoader;
using Terraria.Audio;
using ParticleLibrary;
using Redemption.Particles;
using Redemption.Base;
using System.Collections.Generic;
using Terraria.Graphics.Shaders;

namespace Redemption.Projectiles.Melee
{
    public class Firestorm_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ophos' Firestorm");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjFire[Type] = true;
            ElementID.ProjWind[Type] = true;
        }

        public override bool ShouldUpdatePosition() => false;
        public override void SetDefaults()
        {
            Projectile.width = 124;
            Projectile.height = 210;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 6;
            Projectile.scale = 1.5f;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        private int Frame1;
        private int Frame2 = 1;
        private int Frame3 = 2;
        private int Frame4 = 3;
        private float squish;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            if (player.channel && Projectile.frameCounter++ % 4 == 0)
            {
                if (++Frame1 > 4)
                    Frame1 = 0;
                if (++Frame2 > 4)
                    Frame2 = 0;
                if (++Frame3 > 4)
                    Frame3 = 0;
                if (++Frame4 > 4)
                    Frame4 = 0;
            }
            Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 0, 255);
            if (Main.myPlayer == Projectile.owner)
            {
                if (player.channel && Projectile.ai[1] == 0)
                    Projectile.alpha -= 3;
                else
                    Projectile.alpha += 10;

                if (Main.rand.NextBool(10))
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, new Vector2(Main.rand.Next(-14, 15), -Main.rand.Next(1, 9)), Main.rand.Next(400, 403), Projectile.damage / 3, 1, player.whoAmI);

                if (Projectile.alpha < 50 && !player.channel && Projectile.ai[1] == 0)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC target = Main.npc[i];
                        if (!target.active || !target.CanBeChasedBy())
                            continue;

                        if (target.immune[Projectile.whoAmI] > 0 || Projectile.DistanceSQ(target.Center) > 400 * 400)
                            continue;

                        target.immune[Projectile.whoAmI] = 20;
                        int hitDirection = target.RightOfDir(Projectile);
                        BaseAI.DamageNPC(target, (int)(Projectile.damage * 1.5f), 20, hitDirection, Projectile, crit: Projectile.HeldItemCrit());
                        target.AddBuff(BuffID.OnFire3, 600);
                    }
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, Projectile.position);
                    Projectile.ai[1] = 1;
                }
                if (Projectile.ai[1] > 0)
                {
                    squish += 0.1f;
                    squish *= 0.95f;
                }

                if (Projectile.localAI[0]++ >= 20 && Projectile.alpha >= 255)
                    Projectile.Kill();
            }

            Projectile.Center = player.MountedCenter - new Vector2(0, 96);

            if (Projectile.alpha < 20 && Main.rand.NextBool(3))
                ParticleManager.NewParticle(RedeHelper.RandAreaInEntity(Projectile), new Vector2(Main.rand.Next(-4, 5), 0), new EmberParticle(), Color.White, 2);
        }
        public override bool? CanHitNPC(NPC target) => Projectile.alpha <= 100 ? null : false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 300);
        }
        private float drawTimer;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int width = texture.Width / 6;
            int x = width * Frame1;
            int x2 = width * Frame2;
            int x3 = width * Frame3;
            int x4 = width * Frame3;
            Rectangle rect = new(x, 0, width, texture.Height);
            Rectangle rect2 = new(x2, 0, width, texture.Height);
            Rectangle rect3 = new(x3, 0, width, texture.Height);
            Rectangle rect4 = new(x4, 0, width, texture.Height);
            Vector2 drawOrigin = new(width / 2, texture.Height / 2 + 96);
            Vector2 scale = new(Projectile.scale + squish, Projectile.scale - (squish / 5));

            if (Projectile.alpha <= 0)
            {
                int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.LivingFlameDye);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);
            }
            else
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }
            RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, texture, ref drawTimer, Projectile.Center + new Vector2(0, 96) - Main.screenPosition, new Rectangle?(rect), RedeColor.COLOR_GLOWPULSE * Projectile.Opacity * 0.3f, Projectile.rotation, drawOrigin, scale, 0);
            RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, texture, ref drawTimer, Projectile.Center + new Vector2(0, 96) - Main.screenPosition, new Rectangle?(rect2), RedeColor.COLOR_GLOWPULSE * Projectile.Opacity * 0.3f, Projectile.rotation, drawOrigin, scale, 0);
            RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, texture, ref drawTimer, Projectile.Center + new Vector2(0, 96) - Main.screenPosition, new Rectangle?(rect3), RedeColor.COLOR_GLOWPULSE * Projectile.Opacity * 0.3f, Projectile.rotation, drawOrigin, scale, 0);
            RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, texture, ref drawTimer, Projectile.Center + new Vector2(0, 96) - Main.screenPosition, new Rectangle?(rect4), RedeColor.COLOR_GLOWPULSE * Projectile.Opacity * 0.3f, Projectile.rotation, drawOrigin, scale, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}