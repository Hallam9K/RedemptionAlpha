using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Redemption.Globals;
using System.Collections.Generic;
using Terraria.Audio;
using Redemption.Buffs.Debuffs;
using Terraria.ID;
using Redemption.BaseExtension;

namespace Redemption.NPCs.Soulless
{
    public class TheStalker_Hand : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Handyman's Hand");
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 800;
        }
        public Vector2 origPoint;
        public bool grabbed;
        public bool consumed;
        public int speedUp;
        public bool pause;
        public override void AI()
        {
            if (Projectile.frameCounter++ >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 3)
                    Projectile.frame = 0;
            }
            Player player = Main.player[Projectile.owner];
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active || (npc.type != ModContent.NPCType<TheStalker>() && npc.type != ModContent.NPCType<TheStalker_Fake>()))
            {
                seg.Clear();
                segRot.Clear();
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 10;
            switch (Projectile.ai[1])
            {
                case 1:
                    SoundEngine.PlaySound(CustomSounds.StalkerHandSnap, Projectile.Center);
                    for (int i = 0; i < 6; i++)
                    {
                        origPoint = Projectile.Center;
                        Vector2 offset = new(Main.rand.Next(-2, 3), 22);
                        Projectile.rotation = (origPoint - (origPoint + offset)).ToRotation();
                        Projectile.Center = origPoint + offset;
                        seg.Add(Projectile.Center);
                        segRot.Add(Projectile.rotation);
                        Projectile.localAI[0] = 0;
                    }
                    Projectile.ai[1] = 2;
                    break;
                case 2:
                    Rectangle activeZone = new((449 + SoullessArea.Offset.X) * 16, (1182 + SoullessArea.Offset.Y) * 16, 22 * 16, 5 * 16);
                    if (player.Hitbox.Intersects(activeZone))
                        Projectile.ai[1] = 3;
                    break;
                case 3:
                    if (Projectile.localAI[0]++ >= 2)
                    {
                        if (seg.Count <= 1)
                        {
                            seg.Clear();
                            segRot.Clear();
                            Projectile.Kill();
                        }
                        else
                        {
                            seg.RemoveAt(seg.Count - 1);
                            segRot.RemoveAt(segRot.Count - 1);
                            Projectile.Center = seg[^1];
                            Projectile.rotation = segRot[^1];
                        }
                        SoundEngine.PlaySound(CustomSounds.StalkerHandSnap, Projectile.Center);
                        Projectile.localAI[0] = 0;
                    }
                    break;
                case 4:
                    SoundEngine.PlaySound(CustomSounds.StalkerHandSnap, Projectile.Center);
                    for (int i = 0; i < 6; i++)
                    {
                        origPoint = Projectile.Center;
                        Vector2 offset = new(Main.rand.Next(-2, 3), -22);
                        Projectile.rotation = (origPoint - (origPoint + offset)).ToRotation();
                        Projectile.Center = origPoint + offset;
                        seg.Add(Projectile.Center);
                        segRot.Add(Projectile.rotation);
                        Projectile.localAI[0] = 0;
                    }
                    Projectile.ai[1] = 5;
                    break;
                case 5:
                    activeZone = new((484 + SoullessArea.Offset.X) * 16, (1160 + SoullessArea.Offset.Y) * 16, 47 * 16, 52 * 16);
                    if (player.Hitbox.Intersects(activeZone))
                    {
                        Projectile.ai[1] = 6;
                        SoundStyle s = CustomSounds.SpookyNoise with { Pitch = -.2f };
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(s);

                        player.AddBuff(ModContent.BuffType<StalkerDebuff>(), 1200);
                        SoullessArea.soullessInts[1] = 6;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);
                    }
                    break;
                case 6:
                    if (Projectile.localAI[0]++ >= 2)
                    {
                        if (seg.Count <= 1)
                        {
                            seg.Clear();
                            segRot.Clear();
                            Projectile.Kill();
                        }
                        else
                        {
                            seg.RemoveAt(seg.Count - 1);
                            segRot.RemoveAt(segRot.Count - 1);
                            Projectile.Center = seg[^1];
                            Projectile.rotation = segRot[^1];
                        }
                        SoundEngine.PlaySound(CustomSounds.StalkerHandSnap, Projectile.Center);
                        Projectile.localAI[0] = 0;
                    }
                    break;
            }
            if (Projectile.ai[1] > 0 && Projectile.ai[1] != 10)
                return;
            if (player.whoAmI == Projectile.owner && player.Hitbox.Intersects(Projectile.Hitbox))
            {
                grabbed = true;
                player.Redemption().handymanGrab = true;
            }
            if (grabbed && player.whoAmI == Projectile.owner)
            {
                if (consumed)
                {
                    for (int i = 0; i < Main.musicFade.Length; i++)
                        Main.musicFade[i] = 0f;
                    if (Projectile.localAI[1]++ >= 3)
                    {
                        Main.BlackFadeIn = 500;
                        player.AddBuff(ModContent.BuffType<StunnedDebuff>(), 120);
                    }
                    if (Projectile.localAI[1] >= 180)
                    {
                        Main.musicFade = Redemption.OldMusicFade;
                        SoullessArea.soullessInts[2] = 0;
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.WorldData);
                        player.AddBuff(ModContent.BuffType<StunnedDebuff>(), 120);
                        if (Projectile.ai[1] is 10)
                            player.Center = new Vector2(601 + SoullessArea.Offset.X, 1157 + SoullessArea.Offset.Y) * 16;
                        else
                            player.Center = new Vector2(347 + SoullessArea.Offset.X, 1069 + SoullessArea.Offset.Y) * 16;
                        seg.Clear();
                        segRot.Clear();
                        Projectile.Kill();
                    }
                    return;
                }
                if (Projectile.localAI[0]++ >= 2)
                {
                    if (seg.Count <= 8)
                    {
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            Projectile other = Main.projectile[i];
                            if (!other.active || other.whoAmI == Projectile.whoAmI || other.type != Type)
                                continue;
                            other.Kill();
                        }
                        if (npc.ModNPC is TheStalker stalker)
                            stalker.RAAGH = true;
                        if (npc.ModNPC is TheStalker_Fake stalker2)
                            stalker2.RAAGH = true;
                        Redemption.OldMusicFade = Main.musicFade;
                        SoundEngine.PlaySound(CustomSounds.StalkerScare, player.position);
                        Projectile.Center = npc.Center;
                        consumed = true;
                    }
                    else
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            seg.RemoveAt(seg.Count - 1);
                            segRot.RemoveAt(segRot.Count - 1);
                            Projectile.Center = seg[^1];
                            Projectile.rotation = segRot[^1];
                        }
                    }
                    player.Center = Projectile.Center;
                    SoundEngine.PlaySound(CustomSounds.StalkerHandSnap, Projectile.Center);
                    Projectile.localAI[0] = 0;
                }
                player.velocity *= 0;
                return;
            }
            int speed = 20;
            if (player.DistanceSQ(Projectile.Center) < 100 * 100)
                speed = 40;
            else if (player.DistanceSQ(Projectile.Center) > 400 * 400)
                speed = 10;
            if (player.DistanceSQ(Projectile.Center) > 600 * 600)
            {
                pause = false;
                speed -= 4;
            }
            if (speedUp > 0)
                speed = 4;
            if (Projectile.localAI[0]++ >= speed + (pause ? 40 : 0))
            {
                pause = false;
                speedUp--;
                for (int i = 0; i < 2; i++)
                {
                    origPoint = Projectile.Center;
                    Vector2 offset = RedeHelper.PolarVector(22, (player.Center - origPoint).ToRotation());
                    bool set = false;
                    if (player.DistanceSQ(origPoint) < 500 * 500 && Collision.SolidCollision(new Vector2((origPoint + offset).X - 4, (origPoint + offset).Y - 4), 32, 32))
                    {
                        float rot = 0;
                        int attempts = 0;
                        while (attempts++ < 152 && !set)
                        {
                            rot += .01f;
                            Vector2 shift1 = RedeHelper.PolarVector(22, (player.Center - origPoint).ToRotation() + rot);
                            Vector2 shift2 = RedeHelper.PolarVector(22, (player.Center - origPoint).ToRotation() - rot);
                            if (!Collision.SolidCollision(new Vector2((origPoint + shift1).X - 4, (origPoint + shift1).Y - 4), 32, 32))
                            {
                                Projectile.rotation = (origPoint - (origPoint + shift1)).ToRotation();
                                Projectile.Center = origPoint + shift1;
                                seg.Add(Projectile.Center);
                                segRot.Add(Projectile.rotation);
                                set = true;
                                break;
                            }
                            if (!Collision.SolidCollision(new Vector2((origPoint + shift2).X - 4, (origPoint + shift2).Y - 4), 32, 32))
                            {
                                Projectile.rotation = (origPoint - (origPoint + shift2)).ToRotation();
                                Projectile.Center = origPoint + shift2;
                                seg.Add(Projectile.Center);
                                segRot.Add(Projectile.rotation);
                                set = true;
                                break;
                            }
                        }
                    }
                    if (!set)
                    {
                        if (player.DistanceSQ(Projectile.Center) > 60 * 60)
                            offset = RedeHelper.PolarVector(22, (player.Center - origPoint).ToRotation() + Main.rand.NextFloat(-.2f, .2f));
                        Projectile.rotation = (origPoint - (origPoint + offset)).ToRotation();
                        Projectile.Center = origPoint + offset;
                        seg.Add(Projectile.Center);
                        segRot.Add(Projectile.rotation);
                    }
                    SoundEngine.PlaySound(CustomSounds.StalkerHandSnap, Projectile.Center);
                    Projectile.localAI[0] = 0;
                }
                if (speedUp <= 0 && Main.rand.NextBool(6) && player.DistanceSQ(Projectile.Center) > 200 * 200)
                {
                    pause = true;
                    speedUp = Main.rand.Next(3, 5);
                }
            }
        }
        private readonly List<Vector2> seg = new();
        private readonly List<float> segRot = new();
        public override bool PreDraw(ref Color lightColor)
        {
            if (seg is null || seg.Count <= 0)
                return false;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(Texture + "2").Value;
            Texture2D arm = ModContent.Request<Texture2D>(Texture + "_Arm").Value;
            int height = texture.Height / 4;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2 + 14, height / 2 + 10);
            SpriteEffects effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (seg != null)
            {
                int height2 = arm.Height / 4;
                int y2 = height2 * Projectile.frame;
                Rectangle rect2 = new(0, y2, arm.Width, height2);
                Vector2 drawOrigin2 = new(arm.Width / 2 + 9, height2 / 2 + 9);
                for (int i = 0; i < seg.Count; i++)
                {
                    Main.EntitySpriteDraw(arm, seg[i] - Main.screenPosition, new Rectangle?(rect2), Projectile.GetAlpha(Color.White), segRot[i] - MathHelper.Pi - MathHelper.PiOver4, drawOrigin2, Projectile.scale, effects, 0);
                }
            }
            if (!grabbed)
            {
                Main.EntitySpriteDraw(texture2, Projectile.Center + RedeHelper.Spread(2) - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor) * .5f, Projectile.rotation - .4f, drawOrigin, Projectile.scale, effects, 0);
                Main.EntitySpriteDraw(texture, Projectile.Center + RedeHelper.Spread(2) - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation - .4f, drawOrigin, Projectile.scale, effects, 0);
            }
            return false;
        }
    }
}
