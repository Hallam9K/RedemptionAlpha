using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using ReLogic.Content;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Erhan
{
    public class PalebatImp : ModNPC
    {
        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 17;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 36;
            NPC.height = 42;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 200;
            NPC.HitSound = SoundID.DD2_KoboldHurt;
            NPC.DeathSound = SoundID.DD2_KoboldDeath;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
            if (!NPC.IsABestiaryIconDummy)
                NPC.noGravity = true;
            NPC.dontTakeDamage = true;
            NPC.alpha = 255;
            NPC.boss = true;

            if (!Main.dedServ)
                Music = RedeBossDowned.erhanDeath < 1 ? MusicLoader.GetMusicSlot(Mod, "Sounds/Music/ImpOfDoom") : MusicLoader.GetMusicSlot(Mod, "Sounds/Music/silence");
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 16; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Blood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
                for (int i = 0; i < 30; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Sandnado, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f,
                        Scale: 3);
                    Main.dust[dustIndex].velocity.Y -= 6f;
                    Main.dust[dustIndex].velocity.X *= 0.2f;
                    Main.dust[dustIndex].noGravity = true;
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.None;
        }

        public float shakeTimer;
        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            if (RedeBossDowned.erhanDeath == 0)
            {
                ScreenPlayer.CutsceneLock(Main.LocalPlayer, NPC, ScreenPlayer.CutscenePriority.High, 0, 0, 0);
            }
            switch (TimerRand)
            {
                case 0:
                    if (RedeBossDowned.erhanDeath > 0)
                    {
                        int summon = RedeBossDowned.erhanDeath < 3 ? ModContent.NPCType<Erhan>() : ModContent.NPCType<ErhanSpirit>();
                        RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)player.Center.X + 180, (int)player.Center.Y - 80, summon);
                        NPC.active = false;
                    }
                    else
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            NPC.position = new Vector2(Main.rand.NextBool(2) ? player.Center.X - 180 : player.Center.X + 180, player.Center.Y - 60);
                            NPC.netUpdate = true;
                        }

                        TimerRand = 1;
                    }
                    break;
                case 1:
                    int dustIndex = Dust.NewDust(NPC.BottomLeft + new Vector2(0, 2), NPC.width, 1, DustID.Torch);
                    Main.dust[dustIndex].velocity.Y -= 5f;
                    Main.dust[dustIndex].velocity.X *= 0f;
                    Main.dust[dustIndex].noGravity = true;

                    shakeTimer += 0.004f;
                    shakeTimer = MathHelper.Clamp(shakeTimer, 0, 1.2f);

                    Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity = MathHelper.Max(shakeTimer * 10, Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity);

                    if (AITimer == 80)
                        NPC.alpha = 0;

                    if (AITimer++ == 360)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch, NPC.position);
                        Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 18;
                        DustHelper.DrawDustImage(NPC.Center, DustID.Torch, 0.5f, "Redemption/Effects/DustImages/DemonShape", 3, true, 0);
                    }

                    if (AITimer > 360)
                    {
                        NPC.frame.Y = 12 * 44;
                        AITimer = 0;
                        TimerRand = 2;
                        NPC.netUpdate = true;
                    }
                    break;
                case 2:
                    shakeTimer -= 0.2f;
                    if (AITimer++ == 80)
                    {
                        if (!Main.dedServ)
                            Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/silence");

                        for (int i = 0; i < 20; i++)
                        {
                            int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Torch,
                                NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, Scale: 2);
                            Main.dust[dust].velocity *= 5f;
                            Main.dust[dust].noGravity = true;
                        }
                        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.position);
                        Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 4;
                    }
                    if (AITimer >= 80)
                        NPC.noGravity = false;

                    if (AITimer >= 200)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_GoblinScream, NPC.position);
                        TimerRand = 3;
                        AITimer = 0;
                    }
                    break;
                case 3:
                    if (AITimer++ == 0)
                    {
                        NPC.Shoot(NPC.Center + new Vector2(0, -800), ModContent.ProjectileType<ScorchingRay>(), 0, new Vector2(0, 10), SoundID.Item162);
                    }
                    if (AITimer == 90)
                    {
                        NPC.dontTakeDamage = false;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            NPC.StrikeInstantKill();
                    }
                    break;
            }
        }

        public override void OnKill()
        {
            RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + 180, (int)NPC.Center.Y - 80, ModContent.NPCType<Erhan>());
        }

        public override void FindFrame(int frameHeight)
        {
            if (TimerRand == 1 && AITimer >= 80)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter >= 15)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 11 * frameHeight)
                    {
                        NPC.frame.Y = 11 * frameHeight;
                    }
                }
            }

            if (!NPC.noGravity && NPC.velocity.Y == 0)
            {
                if (TimerRand == 3)
                    NPC.frame.Y = 12 * frameHeight;
                else
                {
                    NPC.frameCounter++;
                    if (NPC.frameCounter >= 6)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 16 * frameHeight)
                            NPC.frame.Y = 13 * frameHeight;
                    }
                }
            }
        }
        Asset<Texture2D> Idle;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                Idle = ModContent.Request<Texture2D>(Texture + "_Idle");
                Rectangle rect = Idle.Frame(1, 4, 0, NPC.frame.Y);
                spriteBatch.Draw(Idle.Value, NPC.Center - screenPos, Idle.Frame(1, 4, 0, NPC.frame.Y), drawColor, 0, rect.Size() / 2, 1, 0, 0);
                return false;
            }
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
    }
}