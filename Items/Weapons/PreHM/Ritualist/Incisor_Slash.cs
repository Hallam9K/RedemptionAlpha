using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Redemption.Globals;
using Terraria.DataStructures;
using Redemption.DamageClasses;
using Redemption.Buffs.NPCBuffs;
using Redemption.BaseExtension;

namespace Redemption.Items.Weapons.PreHM.Ritualist
{
    public class Incisor_Slash : TrueMeleeProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Incisor");
            Main.projFrames[Projectile.type] = 5;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 82;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
            Projectile.DamageType = ModContent.GetInstance<RitualistClass>();
            Projectile.Redemption().RitDagger = true;
        }
        private int directionLock;
        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            directionLock = player.direction;
            player.itemRotation = MathHelper.ToRadians(-90f * player.direction);
            SoundEngine.PlaySound(CustomSounds.Slice3 with { Volume = 0.2f }, Projectile.position);
        }
        private float squish;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.rotation = Projectile.velocity.ToRotation();

            squish += 0.08f;
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();
            if (Main.myPlayer == Projectile.owner)
            {
                player.direction = directionLock;
                player.itemRotation -= MathHelper.ToRadians(-15f * player.direction);
                if (++Projectile.frameCounter >= 2)
                {
                    Projectile.frameCounter = 0;
                    if (Projectile.frame > 1)
                        Projectile.friendly = false;
                    if (Projectile.frame++ > 4)
                        Projectile.Kill();
                }
            }

            Projectile.spriteDirection = player.direction;

            Vector2 Offset = Vector2.Normalize(Projectile.velocity) * 50f;
            Projectile.Center = player.Center + Offset;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            RedeProjectile.Decapitation(target, ref damageDone, ref hit.Crit);
            if (Projectile.type == ModContent.ProjectileType<Incisor_Slash>())
                target.AddBuff(ModContent.BuffType<IncisorDebuff>(), 300);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 5;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            Vector2 scale = new(Projectile.scale + squish, Projectile.scale - squish);
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY,
                new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, scale, effects, 0);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 unit = new Vector2(1.5f, 0).RotatedBy(Projectile.rotation);
            float point = 0f;
            // Run an AABB versus Line check to look for collisions
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - unit * 20,
                Projectile.Center + unit * 34, 22, ref point))
                return true;
            else
                return false;
        }
    }
}