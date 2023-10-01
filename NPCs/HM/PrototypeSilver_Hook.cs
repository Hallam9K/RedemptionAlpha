using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.NPCs.HM
{
    public class PrototypeSilver_Hook : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Prototype Silver");
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.Redemption().TechnicallyMelee = true;
            Projectile.Redemption().ParryBlacklist = true;
            Projectile.Redemption().friendlyHostile = true;
        }
        public override bool? CanHitNPC(NPC target)
        {
            NPC host = Main.npc[(int)Projectile.ai[0]];
            return target == host.Redemption().attacker ? null : false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.FinalDamage *= 4;
        public override void AI()
        {
            NPC host = Main.npc[(int)Projectile.ai[0]];
            if (!host.active || host.ai[0] != 5 || host.type != ModContent.NPCType<PrototypeSilver>())
                Projectile.Kill();

            Vector2 originPos = host.Center + new Vector2(-11 * host.spriteDirection, -9);

            Projectile.timeLeft = 10;

            switch (Projectile.ai[1])
            {
                case 0:
                    Projectile.velocity = RedeHelper.PolarVector(14 + (Projectile.Distance(host.Redemption().attacker.Center) / 30), (host.Redemption().attacker.Center - Projectile.Center).ToRotation());
                    Projectile.ai[1] = 1;
                    break;
                case 1:
                    if (host.Redemption().attacker is Player attackerPlayer && Projectile.Hitbox.Intersects(attackerPlayer.Hitbox))
                        attackerPlayer.position = Projectile.position;
                    Projectile.velocity *= 0.96f;
                    if (Projectile.velocity.Length() < 5)
                    {
                        Projectile.ai[1] = 2;
                        Projectile.netUpdate = true;
                    }
                    break;
                case 2:
                    Projectile.tileCollide = false;
                    if (host.Redemption().attacker is Player attackerPlayer2 && Projectile.Hitbox.Intersects(attackerPlayer2.Hitbox))
                        attackerPlayer2.position = Projectile.position;
                    Projectile.Move(originPos, 20, 20);
                    if (Projectile.DistanceSQ(originPos) < 20 * 20)
                    {
                        host.ai[1] = 100;
                        Projectile.Kill();
                    }
                    break;
            }
            Vector2 position = Projectile.Center;
            Vector2 mountedCenter = originPos;
            Vector2 vector2_4 = mountedCenter - position;
            Projectile.rotation = (float)Math.Atan2(vector2_4.Y, vector2_4.X) - 1.57f;
            Projectile.alpha = host.alpha;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
            Projectile.velocity *= 0;
            return false;
        }
        public override bool CanHitPlayer(Player target) => false;

        public override bool PreDraw(ref Color lightColor)
        {
            NPC host = Main.npc[(int)Projectile.ai[0]];
            Texture2D ballTexture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 anchorPos = Projectile.Center;
            Texture2D chainTexture = ModContent.Request<Texture2D>(Texture + "_Chain").Value;
            Texture2D endTexture = ModContent.Request<Texture2D>(Texture + "_End").Value;
            Vector2 HeadPos = host.Center + new Vector2(-11 * host.spriteDirection, -9);
            Vector2 HeadPosStatic = host.Center + new Vector2(-11 * host.spriteDirection, -9);
            Rectangle sourceRectangle = new(0, 0, chainTexture.Width, chainTexture.Height);
            Vector2 origin = new(chainTexture.Width * 0.5f, chainTexture.Height * 0.5f);
            float num1 = chainTexture.Height;
            Vector2 vector2_4 = anchorPos - HeadPos;
            var effects = host.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float rotation = (float)Math.Atan2(vector2_4.Y, vector2_4.X) - 1.57f;
            bool flag = true;
            if (float.IsNaN(HeadPos.X) && float.IsNaN(HeadPos.Y))
                flag = false;
            if (float.IsNaN(vector2_4.X) && float.IsNaN(vector2_4.Y))
                flag = false;
            while (flag)
            {
                if (vector2_4.Length() < num1 + 1.0)
                    flag = false;
                else
                {
                    Vector2 vector2_1 = vector2_4;
                    vector2_1.Normalize();
                    HeadPos += vector2_1 * num1;
                    vector2_4 = anchorPos - HeadPos;
                    Main.EntitySpriteDraw(chainTexture, HeadPos - Main.screenPosition, new Rectangle?(sourceRectangle), Projectile.GetAlpha(lightColor), rotation, origin, 1, SpriteEffects.None, 0);
                }
            }
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, 0, ballTexture.Width, ballTexture.Height);
            Rectangle rectEnd = new(0, 0, endTexture.Width, endTexture.Height);
            Vector2 origin2 = new(ballTexture.Width / 2, ballTexture.Height / 2);
            Vector2 originEnd = new(endTexture.Width / 2, endTexture.Height / 2);

            Main.EntitySpriteDraw(ballTexture, position, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(endTexture, HeadPosStatic - Main.screenPosition, new Rectangle?(rectEnd), Projectile.GetAlpha(lightColor), Projectile.rotation, originEnd, Projectile.scale, effects, 0);
            return false;
        }
    }
}