using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.KSIII
{
    public class KS3_FlashGrenade : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Stun Grenade");
            Main.projFrames[Projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 24;
            Projectile.penetrate = 1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 90;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.Kill();
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.4f, Projectile.Opacity * 0.4f, Projectile.Opacity);
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                    Projectile.frame = 0;
            }
            Projectile.rotation += Projectile.velocity.X / 40 * Projectile.direction;
            Projectile.velocity.Y += 0.3f;
        }
        public override void OnKill(int timeLeft)
        {
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.Zap2, Projectile.position);
            for (int i = 0; i < 15; i++)
            {
                int dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Frost, 0f, 0f, 100, default, 4f);
                Main.dust[dustIndex].velocity *= 12f;
            }
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FlashGrenadeBlast>(), Projectile.damage, 0, Main.myPlayer);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= 0.98f;
            return false;
        }
    }
    public class FlashGrenadeBlast : ModProjectile
    {
        public override string Texture => "Redemption/Textures/TransitionTex";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Flash");
        }

        public override void SetDefaults()
        {
            Projectile.width = 500;
            Projectile.height = 500;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.alpha = 255;
            Projectile.scale = 2;
        }
        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.alpha -= 10;
                if (Projectile.alpha <= 0)
                    Projectile.localAI[0] = 1;
            }
            else
            {
                if (Projectile.timeLeft < 200)
                    Projectile.alpha += 3;
            }

            if (Projectile.localAI[1]++ <= 6)
            {
                for (int k = 0; k < Main.maxPlayers; k++)
                {
                    Player player = Main.player[k];
                    if (!player.active || player.dead || Projectile.DistanceSQ(player.Center) >= 60 * 60)
                        continue;

                    int hitDirection = player.RightOfDir(Projectile);
                    BaseAI.DamagePlayer(player, Projectile.damage, 3, hitDirection, Projectile);

                    player.AddBuff(BuffID.Confused, 180);
                    player.AddBuff(ModContent.BuffType<StunnedDebuff>(), 60);
                }
            }

            for (int k = 0; k < Main.maxPlayers; k++)
            {
                Player player = Main.player[k];
                if (Projectile.alpha >= 150 || !player.active || player.dead || Projectile.DistanceSQ(player.Center) >= 400 * 400)
                    continue;

                player.AddBuff(BuffID.Obstructed, 10);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
    }
}