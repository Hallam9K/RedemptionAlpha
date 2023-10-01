using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Erhan
{
    public class Erhan_HolyShield : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Holy Shield");
        }
        public override void SetDefaults()
        {
            Projectile.width = 84;
            Projectile.height = 84;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            Player player = Main.player[RedeHelper.GetNearestAlivePlayer(Projectile)];
            NPC host = Main.npc[(int)Projectile.ai[0]];
            if (!host.active || (host.type != ModContent.NPCType<Erhan>() && host.type != ModContent.NPCType<ErhanSpirit>()))
                Projectile.Kill();
            Projectile.timeLeft = 10;
            Projectile.rotation = (host.Center - Projectile.Center).ToRotation();

            if (host.ai[0] != 3)
            {
                Projectile.alpha += 10;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }

            float offset = 0;
            switch (Projectile.ai[1])
            {
                case 1:
                    offset = MathHelper.PiOver2;
                    break;
                case 2:
                    offset = -MathHelper.PiOver2;
                    break;
                case 3:
                    offset = MathHelper.Pi;
                    break;
            }
            Projectile.localAI[0].SlowRotation((player.Center - host.Center).ToRotation() - MathHelper.PiOver4 + offset, (float)Math.PI / 60);
            Projectile.Center = host.Center + Vector2.One.RotatedBy(Projectile.localAI[0]) * 50;

            if (Projectile.alpha < 100)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile target = Main.projectile[i];
                    if (!target.active || target.whoAmI == Projectile.whoAmI || target.hostile || !target.friendly || target.damage > 100)
                        continue;

                    if (target.velocity.Length() == 0 || target.ProjBlockBlacklist() || !Projectile.Hitbox.Intersects(target.Hitbox))
                        continue;

                    SoundEngine.PlaySound(SoundID.Item29, Projectile.position);
                    DustHelper.DrawCircle(target.Center, DustID.GoldFlame, 1, 4, 4, nogravity: true);
                    if (ProjectileID.Sets.CultistIsResistantTo[target.type])
                    {
                        target.Kill();
                        continue;
                    }
                    if (!target.hostile && target.friendly)
                    {
                        target.hostile = true;
                    }
                    target.damage /= 4;
                    target.velocity = -target.velocity;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 0f, 0.15f, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White) * 0.5f, Projectile.rotation, drawOrigin, Projectile.scale + scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
}