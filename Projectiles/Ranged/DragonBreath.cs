using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ranged
{
    public class DragonBreathStart : TrueMeleeProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 16;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center;
            SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { MaxInstances = 2 }, Projectile.position);
            if (Projectile.timeLeft % 4 == 0)
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * 0.8f, ModContent.ProjectileType<DragonBreath>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
        }
    }
    public class DragonBreath : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + 687;
        public float[] oldrot = new float[4];
        public Vector2[] oldPos = new Vector2[4];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjFire[Type] = true;
            ElementID.ProjWind[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = 5;
            Projectile.scale = 0.2f;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 21;
            Projectile.usesLocalNPCImmunity = true;
        }
        public override void AI()
        {
            Projectile.rotation += .1f;
            Projectile.scale += 0.1f;
            Projectile.scale = MathHelper.Clamp(Projectile.scale, 0, 2);
            Projectile.alpha += 8;
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 7)
                    Projectile.frame = 0;
            }

            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
            {
                oldPos[k] = oldPos[k - 1];
                oldrot[k] = oldrot[k - 1];
            }
            oldrot[0] = Projectile.rotation;
            oldPos[0] = Projectile.Center;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (NPCLists.Dragonlike.Contains(target.type))
                modifiers.FinalDamage *= 4;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 120);

            Player player = Main.player[Projectile.owner];
            if (player.RedemptionPlayerBuff().dragonLeadBonus)
                target.AddBuff(ModContent.BuffType<DragonblazeDebuff>(), 300);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.OnFire, 120);
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = texture.Frame(1, 7);
            Vector2 origin = rect.Size() / 2f;
            Color color1 = new(255, 120 + (int)(70 * MathF.Sin(Projectile.timeLeft / 2)), 80);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                Color color = color1 * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, texture.Frame(1, 7, 0, Projectile.frame), Projectile.GetAlpha(color), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(1, 7, 0, Projectile.frame), Projectile.GetAlpha(color1), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
}