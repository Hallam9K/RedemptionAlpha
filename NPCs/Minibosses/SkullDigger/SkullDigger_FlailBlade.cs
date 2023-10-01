using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.NPCs.Minibosses.SkullDigger
{
    public class SkullDigger_FlailBlade : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Skull Digger");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 56;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.Redemption().TechnicallyMelee = true;
        }

        private float rot;
        private float length;
        private float speed;
        public override void AI()
        {
            NPC host = Main.npc[(int)Projectile.ai[0]];
            if (!host.active || host.type != ModContent.NPCType<SkullDigger>())
                Projectile.Kill();

            Vector2 originPos = host.Center + new Vector2(host.spriteDirection == 1 ? 35 : -35, -6);
            Vector2 defaultPosition = new(host.Center.X + (host.spriteDirection == 1 ? 35 : -35), host.Center.Y + 60);
            if (host.alpha >= 255)
                Projectile.Center = defaultPosition;

            Player player = Main.player[host.target];
            Projectile.timeLeft = 10;

            Vector2 position = Projectile.Center;
            Vector2 mountedCenter = originPos;
            Vector2 vector2_4 = mountedCenter - position;
            Projectile.rotation = (float)Math.Atan2(vector2_4.Y, vector2_4.X) + 1.57f;
            Projectile.alpha = host.alpha;

            if (host.ai[0] == 2)
            {
                switch ((host.ModNPC as SkullDigger).ID)
                {
                    case 0:
                        switch (Projectile.localAI[1])
                        {
                            case 0:
                                rot = originPos.ToRotation();
                                length = Projectile.Distance(originPos);
                                speed = MathHelper.ToRadians(2);
                                Projectile.localAI[1] = 1;
                                Projectile.netUpdate = true;
                                break;
                            case 1:
                                Projectile.localAI[0]++;
                                if (Projectile.localAI[0] >= 40 && Projectile.localAI[0] % 20 == 0 && !Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.ChainSwing with { PitchVariance = .1f }, Projectile.position);

                                rot += speed * host.spriteDirection;
                                speed *= 1.04f;
                                speed = MathHelper.Clamp(speed, MathHelper.ToRadians(2), MathHelper.ToRadians(25));
                                Projectile.Center = originPos + new Vector2(0, 1).RotatedBy(rot) * length;
                                if (Projectile.localAI[0] >= 120)
                                {
                                    if (!Main.dedServ)
                                        SoundEngine.PlaySound(CustomSounds.ChainSwing with { PitchVariance = .1f }, Projectile.position);

                                    Projectile.velocity = RedeHelper.PolarVector(14 + (Projectile.Distance(player.Center) / 30), (player.Center - Projectile.Center).ToRotation());
                                    host.velocity = RedeHelper.PolarVector(14, (player.Center - host.Center).ToRotation());
                                    Projectile.localAI[0] = 0;
                                    Projectile.localAI[1] = 2;
                                    Projectile.netUpdate = true;
                                }
                                break;
                            case 2:
                                Projectile.localAI[0]++;
                                Projectile.velocity *= 0.97f;
                                if (Projectile.velocity.Length() < 5)
                                {
                                    Projectile.localAI[1] = 3;
                                    Projectile.netUpdate = true;
                                }
                                break;
                            case 3:
                                Projectile.Move(defaultPosition, 20, 20);
                                if (Projectile.DistanceSQ(defaultPosition) < 50 * 50)
                                {
                                    host.ai[1] = 1;
                                    Projectile.netUpdate = true;
                                }
                                break;
                        }
                        break;
                    case 1:
                        Projectile.localAI[0] = 0;
                        Projectile.localAI[1] = 0;
                        Projectile.Move(defaultPosition, 9, 20);
                        break;
                    case 2:
                        switch (Projectile.localAI[1])
                        {
                            case 0:
                                rot = originPos.ToRotation();
                                length = Projectile.Distance(originPos);
                                speed = MathHelper.ToRadians(1);
                                Projectile.localAI[1] = 1;
                                Projectile.netUpdate = true;
                                break;
                            case 1:
                                Projectile.localAI[0]++;
                                if (Projectile.localAI[0] % 50 == 0 && !Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.ChainSwing with { PitchVariance = .1f }, Projectile.position);

                                length++;
                                length = MathHelper.Clamp(length, 10, 100);
                                rot += speed;
                                speed *= 1.02f;
                                speed = MathHelper.Clamp(speed, MathHelper.ToRadians(2), MathHelper.ToRadians(8));
                                Projectile.Center = originPos + new Vector2(0, 1).RotatedBy(rot) * length;
                                if (Projectile.localAI[0] >= 60 && Projectile.localAI[0] % 15 == 0 && Main.myPlayer == player.whoAmI)
                                {
                                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SkullDigger_FlailBlade_Proj>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.whoAmI, host.whoAmI);
                                }
                                if (Projectile.localAI[0] >= 260)
                                {
                                    Projectile.localAI[0] = 0;
                                    Projectile.localAI[1] = 2;
                                    Projectile.netUpdate = true;
                                }
                                break;
                            case 2:
                                Projectile.Move(defaultPosition, 20, 20);
                                if (Projectile.DistanceSQ(defaultPosition) < 50 * 50)
                                {
                                    host.ai[1] = 1;
                                    Projectile.netUpdate = true;
                                }
                                break;
                        }
                        break;
                }
            }
            else
            {
                Projectile.localAI[0] = 0;
                Projectile.localAI[1] = 0;
                Projectile.Move(defaultPosition, 9, 20);
            }
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            NPC host = Main.npc[(int)Projectile.ai[0]];
            if (host.RedemptionNPCBuff().disarmed)
                modifiers.FinalDamage *= 0.66f;
        }

        public override bool CanHitPlayer(Player target)
        {
            NPC host = Main.npc[(int)Projectile.ai[0]];
            return host.ai[0] == 2 && (host.ModNPC as SkullDigger).ID != 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            NPC host = Main.npc[(int)Projectile.ai[0]];
            Texture2D ballTexture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 anchorPos = Projectile.Center;
            Texture2D chainTexture = ModContent.Request<Texture2D>("Redemption/NPCs/Minibosses/SkullDigger/SkullDigger_Chain").Value;
            Vector2 HeadPos = host.Center + new Vector2(host.spriteDirection == 1 ? 35 : -35, -6);
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
            Vector2 origin2 = new(ballTexture.Width / 2, ballTexture.Height / 2);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] + Projectile.Size / 2f - Main.screenPosition;
                Color color = Projectile.GetAlpha(Color.LightCyan) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(ballTexture, drawPos, new Rectangle?(rect), color, Projectile.rotation, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(ballTexture, position, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}
