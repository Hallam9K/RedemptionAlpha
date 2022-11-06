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

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class ForgottenSword_Proj : TrueMeleeProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sword of the Forgotten");
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void SetSafeDefaults()
        {
            Projectile.width = 192;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
        }
        private int dir = 1;
        private int heatFrame = 1;
        public override void AI()
        {
            if (Projectile.frameCounter++ % 3 == 0)
            {
                if (++Projectile.frame > 2)
                    Projectile.frame = 0;
                if (++heatFrame > 2)
                    heatFrame = 0;
            }
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            player.direction = dir;
            Projectile.spriteDirection = player.direction;

            if (Main.myPlayer == Projectile.owner)
            {
                if (Projectile.localAI[0]++ == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item1, player.Center);
                    dir = player.direction;
                }
                if (Projectile.localAI[0] >= 10)
                {
                    SoundEngine.PlaySound(SoundID.Item1, player.Center);
                    dir *= -1;
                    Projectile.localAI[0] = 1;
                }
                if (Projectile.localAI[1] % 50 == 0)
                {
                    SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack with { Pitch = -0.1f }, player.Center);
                }

                Player.CompositeArmStretchAmount arm = Player.CompositeArmStretchAmount.Full;
                if (Projectile.localAI[0] >= 4 && Projectile.localAI[0] < 7)
                    arm = Player.CompositeArmStretchAmount.ThreeQuarters;
                else if (Projectile.localAI[0] >= 7 && Projectile.localAI[0] < 9)
                    arm = Player.CompositeArmStretchAmount.ThreeQuarters;

                player.SetCompositeArmFront(true, arm, MathHelper.PiOver2);

                if (Main.rand.NextBool(20))
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, new Vector2(Main.rand.Next(-8, 9), -Main.rand.Next(1, 5)), Main.rand.Next(400, 403), Projectile.damage / 3, 1, player.whoAmI);

                if (Projectile.localAI[1]++ >= 20 && !player.channel)
                    Projectile.Kill();
            }

            Projectile.Center = player.MountedCenter;

            if (Main.rand.NextBool(8))
                ParticleManager.NewParticle(RedeHelper.RandAreaInEntity(Projectile), RedeHelper.Spread(2), new EmberParticle(), Color.White, 1);
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 260);
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            RedeProjectile.Decapitation(target, ref damage, ref crit);
        }
        private float drawTimer;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "2").Value;
            int height = texture.Height / 3;
            int y = height * Projectile.frame;
            int y2 = height * heatFrame;
            Rectangle rect = new(0, y, texture.Width, height);
            Rectangle rect2 = new(0, y2, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, height / 2);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + Vector2.UnitY * Projectile.gfxOffY;
                Color color = RedeColor.FadeColour1 * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(glow, drawPos, new Rectangle?(rect2), Projectile.GetAlpha(color) * 2f, Projectile.rotation, drawOrigin, Projectile.scale, 0, 0);
            }
            RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, glow, ref drawTimer, Projectile.Center - Main.screenPosition, new Rectangle?(rect2), RedeColor.COLOR_GLOWPULSE * Projectile.Opacity * 0.4f, Projectile.rotation, drawOrigin, Projectile.scale, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}