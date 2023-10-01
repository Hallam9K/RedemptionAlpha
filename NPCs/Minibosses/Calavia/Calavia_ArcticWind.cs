using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.NPCs.Friendly.SpiritSummons;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Minibosses.Calavia
{
    public class Calavia_ArcticWind : ModProjectile
    {
        public override string Texture => "Redemption/Textures/IceFlake";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Arctic Wind");
        }
        public override void SetDefaults()
        {
            Projectile.width = 160;
            Projectile.height = 160;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 120;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active || npc.ai[0] is 4 || (npc.type != ModContent.NPCType<Calavia>() && npc.type != ModContent.NPCType<Calavia_SS>()))
                Projectile.Kill();

            Projectile.rotation += 0.1f * npc.direction;
            Projectile.Center = npc.Center;

            for (int i = 0; i < 30; i++)
            {
                float distance = Main.rand.Next(25) * 4;
                Vector2 dustRotation = new Vector2(distance, 0f).RotatedBy(MathHelper.ToRadians(i * 12));
                Vector2 dustPosition = Projectile.Center + dustRotation;
                Vector2 nextDustPosition = Projectile.Center + dustRotation.RotatedBy(MathHelper.ToRadians(-4));
                Vector2 dustVelocity = (dustPosition - nextDustPosition + Projectile.velocity) * npc.direction;
                if (Main.rand.NextBool(10))
                {
                    Dust dust = Dust.NewDustPerfect(dustPosition, DustID.IceTorch, dustVelocity, 0, Scale: 1.5f);
                    dust.noGravity = true;
                    dust.noLight = true;
                    dust.alpha += 10;
                    dust.rotation = dustRotation.ToRotation();
                }
            }
            for (int i = 0; i < 10; i++)
            {
                float distance = Main.rand.Next(25) * 4;
                Vector2 dustRotation = new Vector2(distance, 0f).RotatedBy(MathHelper.ToRadians(i * 36));
                Vector2 dustPosition = Projectile.Center + dustRotation;
                Vector2 nextDustPosition = Projectile.Center + dustRotation.RotatedBy(MathHelper.ToRadians(-4));
                Vector2 dustVelocity = (dustPosition - nextDustPosition + Projectile.velocity) * npc.direction;
                if (Main.rand.NextBool(30))
                {
                    Dust dust = Dust.NewDustPerfect(dustPosition, ModContent.DustType<SnowflakeDust>(), dustVelocity, 0);
                    dust.noGravity = true;
                    dust.noLight = true;
                    dust.alpha += 10;
                    dust.rotation = dustRotation.ToRotation();
                }
            }

            if (Projectile.timeLeft >= 110)
                Projectile.alpha -= 10;

            if (Projectile.timeLeft <= 20)
            {
                Projectile.alpha += 3;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
                return;
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || !target.CanBeChasedBy())
                    continue;

                if (Projectile.DistanceSQ(target.Center) > 170 * 170)
                    continue;

                target.AddBuff(ModContent.BuffType<PureChillDebuff>(), 300);
                if (Projectile.DistanceSQ(target.Center) > 140 * 140 || target.knockBackResist is 0 || target.RedemptionNPCBuff().iceFrozen)
                    continue;
                target.AddBuff(ModContent.BuffType<IceFrozen>(), 1000 - ((int)MathHelper.Clamp(npc.lifeMax, 60, 980)));
            }
            if (npc.type != ModContent.NPCType<Calavia>())
                return;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player target = Main.player[i];
                if (!target.active || target.dead)
                    continue;

                if (Projectile.DistanceSQ(target.Center) > 170 * 170)
                    continue;
                target.AddBuff(ModContent.BuffType<PureChillDebuff>(), 300);
                if (Projectile.DistanceSQ(target.Center) > 140 * 140)
                    continue;
                target.AddBuff(BuffID.Frozen, 60);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            SpriteEffects effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale * 4, effects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
}
