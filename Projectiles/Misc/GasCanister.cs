using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
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
            DisplayName.SetDefault("Gas Canister");
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 190;
        }
        public override void AI()
        {
            Projectile.LookByVelocity();
            Projectile.rotation += Projectile.velocity.X / 20;
            Projectile.velocity.Y += 0.2f;
        }
        public override void Kill(int timeLeft)
        {
            Vector2 spawn = new Vector2(Projectile.Center.X, Projectile.Center.Y - 100);
            if (!Main.dedServ)
                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Gas1"), Projectile.position);

            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), spawn, Vector2.Zero, ModContent.ProjectileType<GasCanister_Gas>(), 0, 0, Projectile.owner);
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
            DisplayName.SetDefault("Infectious Gas");
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
            Projectile.rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
            Projectile.GetGlobalProjectile<RedeProjectile>().Unparryable = true;
        }

        public override void AI()
        {      
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
                    if (!target.active || target.friendly)
                        continue;

                    if (!Projectile.Hitbox.Intersects(target.Hitbox))
                        continue;

                    target.AddBuff(ModContent.BuffType<VirulityDebuff>(), 420);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.Green), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
    }
}