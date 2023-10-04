using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria.GameContent;
using Redemption.BaseExtension;
using Terraria.Audio;
using ReLogic.Content;
using Redemption.Globals.NPC;

namespace Redemption.NPCs.Bosses.Gigapora
{
    public class Porakone : ModNPC
    {
        private static Asset<Texture2D> glowMask;
        private static Asset<Texture2D> boosterAni;
        private static Asset<Texture2D> boosterGlow;
        private static Asset<Texture2D> poraAni;
        private static Asset<Texture2D> poraGlow;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            glowMask = ModContent.Request<Texture2D>(Texture + "_Glow");
            boosterAni = ModContent.Request<Texture2D>(Texture + "_Booster");
            boosterGlow = ModContent.Request<Texture2D>(Texture + "_Booster_Glow");
            poraAni = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Gigapora/Porakone");
            poraGlow = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Gigapora/Porakone_Glow");
        }
        public override void Unload()
        {
            glowMask = null;
            boosterAni = null;
            boosterGlow = null;
            poraAni = null;
            poraGlow = null;
        }
        public override string Texture => "Redemption/NPCs/Bosses/Cleaver/Wielder";
        public enum ActionState
        {
            Begin,
            Intro
        }
        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }
        public ref float AITimer => ref NPC.ai[2];
        public ref float AITimer2 => ref NPC.ai[3];
        public float[] oldrot = new float[5];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wielder Bot");
            Main.npcFrameCount[NPC.type] = 19;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 32;
            NPC.friendly = false;
            NPC.damage = 0;
            NPC.defense = 2;
            NPC.lifeMax = 250;
            NPC.npcSlots = 10f;
            NPC.SpawnWithHigherTime(30);
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.chaseable = false;
            NPC.dontTakeDamage = true;
        }
        public override bool CheckActive()
        {
            Player player = Main.player[NPC.target];
            return !player.active || player.dead || Main.dayTime;
        }

        public int aniType;
        public int boosterFrame;
        public override void AI()
        {
            if (NPC.DespawnHandler())
                return;
            Player player = Main.player[NPC.target];

            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            if (!player.active || player.dead)
                return;

            switch (AIState)
            {
                case ActionState.Begin:
                    {
                        ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Medium, 1200, 2400, 0);
                        if (AITimer++ == 0)
                        {
                            aniType = 1;
                            NPC.velocity.X = 2;
                            NPC.velocity.Y = 8;
                        }
                        else if (AITimer >= 30)
                            NPC.Move(player.Center + new Vector2(player.Center.X < NPC.Center.X ? 100 : -100, 80), 8, 40);

                        NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
                        if (NPC.Center.Y > player.Center.Y - 80 || NPC.ai[2] > 200)
                        {
                            NPC.velocity *= 0.94f;
                            if (NPC.velocity.Length() < 3)
                            {
                                NPC.rotation = 0;
                                NPC.LookAtEntity(player);
                                aniType = 0;
                                NPC.velocity *= 0;
                                NPC.netUpdate = true;
                                AITimer = 0;
                                AIState = ActionState.Intro;
                            }
                        }
                    }
                    break;
                case ActionState.Intro:
                    ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Medium, 1200, 2400, 0);
                    if (AITimer++ == 60)
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.ShootChange, NPC.position);
                        aniType = 2;
                        NPC.netUpdate = true;
                    }
                    if (AITimer == 120 || AITimer == 150)
                        SoundEngine.PlaySound(SoundID.Item23, NPC.position);

                    if (AITimer >= 180)
                    {
                        AITimer2 += 0.01f;
                        Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity = MathHelper.Max(AITimer2 * 15, Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity);
                        if (NPC.soundDelay == 0)
                        {
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.Quake with { Volume = MathHelper.Clamp(AITimer2, 0.1f, 2f) }, NPC.position);

                            NPC.soundDelay = 50;
                        }
                    }
                    if (AITimer >= 240 && !NPC.AnyNPCs(ModContent.NPCType<Gigapora>()))
                        RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + 280, (int)NPC.Center.Y + 1400, ModContent.NPCType<Gigapora>());

                    int n = NPC.FindFirstNPC(ModContent.NPCType<Gigapora>());
                    if (n != -1)
                    {
                        NPC giga = Main.npc[n];
                        bool gigaHitbox = giga.active && giga.Hitbox.Intersects(NPC.Hitbox);
                        if (AITimer >= 400 || gigaHitbox)
                        {
                            NPC.active = false;
                            SoundEngine.PlaySound(SoundID.NPCDeath14, NPC.position);
                            for (int i = 0; i < 10; i++)
                                Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.LifeDrain, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

                            if (Main.netMode == NetmodeID.Server)
                                return;

                            for (int i = 0; i < 4; i++)
                                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/PorakoneGore" + (i + 1)).Type, 1);
                        }
                    }
                    break;
            }
            if (NPC.ai[1] != 2)
            {
                int dustIndex = Dust.NewDust(new Vector2(NPC.position.X + 3, NPC.position.Y + 16), 10, 2, DustID.LifeDrain, 0, 0, 0, default, 1f);
                Main.dust[dustIndex].noGravity = true;
                Dust dust = Main.dust[dustIndex];
                dust.velocity.Y = 3;
                dust.velocity.X = 0;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            switch (aniType)
            {
                case 0:
                    NPC.frameCounter++;
                    if (NPC.velocity.Length() == 0)
                        NPC.frame.Y = 0;
                    else
                        NPC.frame.Y = frameHeight;
                    break;
                case 1: // Speen
                    if (NPC.frame.Y < 11 * frameHeight)
                        NPC.frame.Y = 11 * frameHeight;

                    NPC.frameCounter++;
                    if (NPC.frameCounter >= 5)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y >= 18 * frameHeight)
                            NPC.frame.Y = 11 * frameHeight;
                    }
                    break;
            }
            if (NPC.frameCounter % 5 == 0)
            {
                boosterFrame++;
            }
            if (boosterFrame >= 4)
                boosterFrame = 0;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int num214 = boosterAni.Value.Height / 4;
            int y6 = num214 * boosterFrame;
            spriteBatch.Draw(boosterAni.Value, NPC.Center - screenPos, new Rectangle?(new Rectangle(0, y6, boosterAni.Value.Width, num214)), drawColor, NPC.rotation, new Vector2(boosterAni.Value.Width / 2f, num214 / 2f), NPC.scale, effects, 0);
            spriteBatch.Draw(boosterGlow.Value, NPC.Center - screenPos, new Rectangle?(new Rectangle(0, y6, boosterAni.Value.Width, num214)), RedeColor.RedPulse, NPC.rotation, new Vector2(boosterAni.Value.Width / 2f, num214 / 2f), NPC.scale, effects, 0);

            if (aniType == 2)
            {
                spriteBatch.Draw(poraAni.Value, NPC.Center - new Vector2(-18 * NPC.spriteDirection, 9) - screenPos, null, drawColor, NPC.rotation, new Vector2(poraAni.Value.Width / 2f, poraAni.Value.Height / 2f), NPC.scale, effects, 0);
                spriteBatch.Draw(poraGlow.Value, NPC.Center - new Vector2(-18 * NPC.spriteDirection, 9) - screenPos, null, RedeColor.RedPulse, NPC.rotation, new Vector2(poraAni.Value.Width / 2f, poraAni.Value.Height / 2f), NPC.scale, effects, 0);
            }
            else
            {
                spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
                spriteBatch.Draw(glowMask.Value, NPC.Center - screenPos, NPC.frame, RedeColor.RedPulse, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            }
            return false;
        }
    }
}