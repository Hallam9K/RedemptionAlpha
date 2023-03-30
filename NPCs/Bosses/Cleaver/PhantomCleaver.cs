using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Terraria.GameContent;
using Terraria.ID;

namespace Redemption.NPCs.Bosses.Cleaver
{
    public class PhantomCleaver : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Phantom Cleaver");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 98;
            Projectile.height = 280;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }
        public float rot;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.5f, 0f, 0f);
            switch (Projectile.localAI[0])
            {
                case 0:
                    Projectile.rotation = Projectile.DirectionTo(player.Center).ToRotation() + 1.57f;
                    rot = Projectile.rotation;
                    Projectile.alpha -= 5;
                    Projectile.velocity *= 0.97f;
                    if (Projectile.velocity.Length() < 1 && Projectile.alpha <= 0)
                    {
                        Projectile.velocity *= 0;
                        Projectile.localAI[0] = 1;
                    }
                    break;
                case 1:
                    Projectile.hostile = true;
                    Projectile.rotation = rot;
                    rot.SlowRotation(Projectile.velocity.ToRotation() + 1.57f, (float)Math.PI / 30f);
                    if (Projectile.localAI[1] == 0)
                    {
                        Projectile.velocity = Projectile.DirectionTo(player.Center) * 30;
                        Projectile.localAI[1] = 1;
                    }
                    Projectile.alpha += 3;
                    if (Projectile.alpha >= 255)
                        Projectile.Kill();
                    break;
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<SnippedDebuff>(), Main.expertMode ? 400 : 200);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 unit = new Vector2(1.5f, 0).RotatedBy(Projectile.rotation + -MathHelper.PiOver2);
            float point = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - unit * 72,
                Projectile.Center + unit * 72, 58, ref point))
                return true;
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin;
                Color color = Projectile.GetAlpha(RedeColor.RedPulse) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(RedeColor.RedPulse), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
    public class PhantomCleaver_F : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Cleaver/PhantomCleaver";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Phantom Cleaver");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 98;
            Projectile.height = 280;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }

        public float rot;
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.5f, 0f, 0f);
            switch (Projectile.localAI[0])
            {
                case 0:
                    Projectile.rotation = Projectile.DirectionTo(Main.MouseWorld).ToRotation() + 1.57f;
                    rot = Projectile.rotation;
                    Projectile.alpha -= 5;
                    Projectile.velocity *= 0.97f;
                    if (Projectile.velocity.Length() < 1 && Projectile.alpha <= 0)
                    {
                        Projectile.velocity *= 0;
                        Projectile.localAI[0] = 1;
                    }
                    break;
                case 1:
                    Projectile.friendly = true;
                    Projectile.rotation = rot;
                    rot.SlowRotation(Projectile.velocity.ToRotation() + 1.57f, (float)Math.PI / 30f);
                    if (Projectile.localAI[1] == 0)
                    {
                        Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * 30;
                        Projectile.localAI[1] = 1;
                    }
                    Projectile.alpha += 3;
                    if (Projectile.alpha >= 255)
                        Projectile.Kill();
                    break;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 unit = new Vector2(1.5f, 0).RotatedBy(Projectile.rotation + -MathHelper.PiOver2);
            float point = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - unit * 72,
                Projectile.Center + unit * 72, 58, ref point))
                return true;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin;
                Color color = Projectile.GetAlpha(RedeColor.RedPulse) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(RedeColor.RedPulse), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
    public class PhantomCleaver2_Spawner : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Phantom Cleaver");
        }
        public override void SetDefaults()
        {
            Projectile.width = 98;
            Projectile.height = 280;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.timeLeft = 16;
        }
        public override void AI()
        {
            if (Projectile.localAI[0] % 3 == 0 && Main.myPlayer == Projectile.owner)
            {
                Projectile.localAI[1] += 5;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(Projectile.localAI[1], 0), ModContent.ProjectileType<PhantomCleaver2>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(-Projectile.localAI[1], 0), ModContent.ProjectileType<PhantomCleaver2>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
    }
    public class PhantomCleaver2 : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Cleaver/PhantomCleaver";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Phantom Cleaver");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 98;
            Projectile.height = 280;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }
        public float rot;
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.5f, 0f, 0f);
            Projectile.rotation = (float)Math.PI;
            switch (Projectile.localAI[0])
            {
                case 0:
                    Projectile.alpha -= 5;
                    Projectile.velocity *= 0.97f;
                    if (Projectile.velocity.Length() < 1 && Projectile.alpha <= 0)
                    {
                        Projectile.velocity *= 0;
                        Projectile.localAI[0] = 1;
                    }
                    break;
                case 1:
                    Projectile.hostile = true;
                    if (Projectile.localAI[1] == 0)
                    {
                        Projectile.velocity.Y = 20;
                        Projectile.localAI[1] = 1;
                    }
                    Projectile.alpha += 3;
                    if (Projectile.alpha >= 255)
                        Projectile.Kill();
                    break;
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<SnippedDebuff>(), Main.expertMode ? 400 : 200);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 unit = new Vector2(1.5f, 0).RotatedBy(Projectile.rotation + -MathHelper.PiOver2);
            float point = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - unit * 72,
                Projectile.Center + unit * 72, 58, ref point))
                return true;
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin;
                Color color = Projectile.GetAlpha(RedeColor.RedPulse) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(RedeColor.RedPulse), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}