using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Misc
{
    public class GasCanister : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Virulent Grenade");
            Main.projFrames[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 360;
        }
        public override void AI()
        {
            Vector2 spawn = new(Projectile.Center.X, Projectile.Center.Y - 100);
            Projectile.LookByVelocity();
            Projectile.rotation += Projectile.velocity.X / 20;
            Projectile.velocity.Y += 0.2f;

            Projectile.localAI[0]++;

            if (Projectile.localAI[0] == 180)
            {
                Projectile.frame = 1;
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position + new Vector2(13, 2), RedeHelper.SpreadUp(5),
                        ModContent.Find<ModGore>("Redemption/GasCanister1").Type);
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position + new Vector2(-13, 2), RedeHelper.SpreadUp(5),
                        ModContent.Find<ModGore>("Redemption/GasCanister2").Type);
                }

                if (!Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.Gas1, Projectile.position);

                if (Projectile.owner == Main.myPlayer)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), spawn, Vector2.Zero, ModContent.ProjectileType<GasCanister_Gas>(), 0, 0, Projectile.owner);
            }

            if (Projectile.localAI[0] >= 300)
            {
                Projectile.alpha += 5;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                if (oldVelocity.X > 4 || oldVelocity.X < -4)
                    SoundEngine.PlaySound(SoundID.Tink, Projectile.position);

                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                if (oldVelocity.Y > 4 || oldVelocity.Y < -4)
                    SoundEngine.PlaySound(SoundID.Tink, Projectile.position);

                Projectile.velocity.Y = -oldVelocity.Y;
            }
            Projectile.velocity.Y *= 0.2f;
            Projectile.velocity.X *= 0.5f;
            return false;
        }
    }

    public class GasCanister_Gas : ModProjectile
    {
        public override string Texture => "Redemption/Textures/IceMist";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Virulent Gas");
        }
        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 150;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.timeLeft = 240;
            Projectile.scale = Main.rand.NextFloat(2, 2.5f);
            Projectile.rotation = RedeHelper.RandomRotation();
        }
        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
                Projectile.localAI[0] = Main.rand.Next(1, 3);

            if (Projectile.localAI[0] == 1)
                Projectile.rotation -= 0.003f;
            else if (Projectile.localAI[0] == 2)
                Projectile.rotation += 0.003f;

            if (Projectile.timeLeft < 80)
            {
                Projectile.alpha += 20;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            else
            {
                Projectile.alpha -= 5;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC target = Main.npc[i];
                    if (!target.active || !target.CanBeChasedBy())
                        continue;

                    if (!Projectile.Hitbox.Intersects(target.Hitbox))
                        continue;

                    target.AddBuff(ModContent.BuffType<ViralityDebuff>(), 420);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.ForestGreen), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
    }
}