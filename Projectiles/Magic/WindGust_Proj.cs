using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class WindGust_Proj : ModProjectile
    {
        public Vector2[] oldPos = new Vector2[10];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            ElementID.ProjWind[Type] = true;
            ElementID.ProjArcane[Type] = true;
            ProjectileID.Sets.DontCancelChannelOnKill[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 94;
            Projectile.height = 94;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 4;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.timeLeft = 180;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.extraUpdates = 1;
        }

        public bool WaterGust
        {
            get => Projectile.ai[2] != 0;
            set => Projectile.ai[2] = value ? 1 : 0;
        }

        public bool SwapRotation
        {
            get => Projectile.ai[1] != 0;
            set => Projectile.ai[1] = value ? 1 : 0;
        }

        bool onSpawn;
        public override void AI()
        {
            if (!onSpawn)
            {
                if (WaterGust)
                {
                    SoundEngine.PlaySound(SoundID.Splash, Projectile.Center);
                    Projectile.GetGlobalProjectile<ElementalProjectile>().OverrideElement[ElementID.Water] = 1;
                }

                onSpawn = true;
                Projectile.netUpdate = true;
            }

            Projectile.ai[0]++;
            if (Projectile.ai[0] % 30 == 0)
            {
                SwapRotation = !SwapRotation;
                Projectile.netUpdate = true;
            }

            if (Collision.SolidCollision(Projectile.Center - new Vector2(4, 4), 8, 8))
            {
                if (Projectile.timeLeft > 30)
                    Projectile.timeLeft = 30;
                Projectile.velocity *= .9f;
            }
            else
            {
                float rot = 0.005f;
                if (Projectile.ai[0] >= 30)
                    rot = 0.01f * (Projectile.ai[0] / 30);
                Projectile.velocity = Projectile.velocity.RotatedBy(SwapRotation ? rot : -rot);
                if (Projectile.velocity.Length() <= 9)
                    Projectile.velocity *= 1.05f;
            }

            if (WaterGust && Projectile.alpha <= 250)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Water, Projectile.velocity.X, Projectile.velocity.Y, Scale: 1.5f);
                Main.dust[d].noGravity = true;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Projectile.timeLeft < 60)
                Projectile.alpha = (int)MathHelper.Lerp(255, 220, Projectile.timeLeft / 60f);
            else if (Projectile.timeLeft >= 180 - 60)
                Projectile.alpha = (int)MathHelper.Lerp(220, 255, (Projectile.timeLeft - 180) / 60f);

            if (Projectile.timeLeft % 2 == 0)
            {
                for (int k = oldPos.Length - 1; k > 0; k--)
                    oldPos[k] = oldPos[k - 1];
                oldPos[0] = Projectile.Center;
            }
        }
        public override bool? CanHitNPC(NPC target) => Projectile.alpha <= 250 ? null : false;
        public override bool CanHitPlayer(Player target) => Projectile.alpha <= 250;

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (!WaterGust || target.noTileCollide || BaseAI.HitTileOnSide(target, 3, false))
                return;
            modifiers.DisableKnockback();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!WaterGust || target.noTileCollide || !BaseAI.HitTileOnSide(target, 3, false))
                return;

            SoundEngine.PlaySound(SoundID.Item21, target.Bottom);
            if (target.knockBackResist > 0)
                target.velocity.Y -= 40 * (target.knockBackResist / 4);

            for (int i = 0; i < 30; i++)
            {
                int d = Dust.NewDust(target.Bottom - new Vector2(10, 10), 20, 20, DustID.Water, Main.rand.Next(-8, 9), -15, Scale: 2);
            }
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 32;
            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = TextureAssets.Projectile[Type];
            Vector2 drawOrigin = texture.Size() / 2;
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Color color = lightColor;
            if (WaterGust)
                color = Lighting.GetColor(Projectile.Center.ToTileCoordinates(), Color.LightBlue);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = oldPos[k] - Main.screenPosition;
                Color color2 = Projectile.GetAlpha(color) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture.Value, drawPos, null, color2 * .7f, Projectile.oldRot[k], drawOrigin, Projectile.scale, effects, 0);
            }
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(color), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
    }
}