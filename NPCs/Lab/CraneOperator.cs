using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Redemption.NPCs.Lab.MACE;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.Utilities;
using Redemption.Globals;
using Redemption.WorldGeneration;
using Redemption.Biomes;

namespace Redemption.NPCs.Lab
{
    public class CraneOperator : ModNPC
    {
        public static int BodyType() => ModContent.NPCType<MACEProject>();
        public enum ActionState
        {
            Idle,
            Typing,
            Slam,
            SlamLeft,
            SlamRight,
            Headbutt,
            Kick,
            Defeat,
            WalkAway
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 82;
            NPC.height = 56;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 250;
            NPC.aiStyle = -1;
            NPC.value = 0f;
            NPC.knockBackResist = 0;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.dontTakeDamage = true;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<LidenBiomeOmega>().Type, ModContent.GetInstance<LabBiome>().Type };
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = BodyType();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement("A timid android with ambitious goals. She does small, odd jobs around the Laboratory. Usually ones that required an operator.")
            });
        }
        private Vector2 moveTo;
        public override void AI()
        {
            if (!NPC.AnyNPCs(BodyType()))
            {
                AITimer = 0;
                if (!RedeBossDowned.downedMACE)
                {
                    AniFrameX = 0;
                    AniFrameY = 0;
                    AIState = ActionState.Idle;
                }
                else if (AIState < ActionState.Defeat)
                {
                    AniFrameY = 12;
                    AniFrameX = 1;
                    AIState = ActionState.Defeat;
                }
            }
            switch (AIState)
            {
                case ActionState.Idle:
                    if (NPC.AnyNPCs(BodyType()))
                    {
                        NPC.frameCounter = 0;
                        AIState = ActionState.Typing;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Typing:
                    if (Main.rand.NextBool(100))
                    {
                        NPC.frameCounter = 0;
                        switch (Main.rand.Next(5))
                        {
                            case 0:
                                AniFrameX = 0;
                                AniFrameY = 11;
                                AIState = ActionState.Slam;
                                break;
                            case 1:
                                AniFrameY = 15;
                                AniFrameX = 1;
                                AIState = ActionState.SlamLeft;
                                break;
                            case 2:
                                AniFrameY = 14;
                                AniFrameX = 0;
                                AIState = ActionState.SlamRight;
                                break;
                            case 3:
                                AniFrameY = 0;
                                AniFrameX = 0;
                                AIState = ActionState.Kick;
                                break;
                            case 4:
                                AniFrameX = 1;
                                AniFrameY = 0;
                                AIState = ActionState.Headbutt;
                                break;

                        }
                    }
                    break;
                case ActionState.WalkAway:
                    if (NPC.DistanceSQ(moveTo) < 16 * 16)
                    {
                        for (int g = 0; g < 4; g++)
                        {
                            int goreIndex = Gore.NewGore(NPC.Center, default, Main.rand.Next(61, 64), 2f);
                            Main.gore[goreIndex].velocity.X += 1.5f;
                            Main.gore[goreIndex].velocity.Y += 1.5f;
                        }
                        NPC.alpha = 255;
                        NPC.active = false;
                    }
                    else
                    {
                        moveTo = new((RedeGen.LabVector.X + 114) * 16, (RedeGen.LabVector.Y + 156) * 16);

                        bool jumpDownPlatforms = false;
                        NPC.JumpDownPlatform(ref jumpDownPlatforms, 20);
                        if (jumpDownPlatforms) { NPC.noTileCollide = true; }
                        else { NPC.noTileCollide = false; }
                        RedeHelper.HorizontallyMove(NPC, moveTo, 0.4f, 1f, 8, 8, NPC.Center.Y > moveTo.Y);
                    }
                    break;
            }
        }
        private int AniFrameY;
        private int AniFrameX;
        public override void FindFrame(int frameHeight)
        {
            switch (AIState)
            {
                case ActionState.Idle:
                    if (++NPC.frameCounter >= 6)
                    {
                        NPC.frameCounter = 0;
                        if (TimerRand == 1)
                        {
                            NPC.frame.Y += frameHeight;
                            if (NPC.frame.Y > 3 * frameHeight)
                            {
                                TimerRand = 0;
                                NPC.frame.Y = 0 * frameHeight;
                            }
                        }
                        else if (Main.rand.NextBool(20))
                            TimerRand = 1;
                    }
                    break;
                case ActionState.Typing:
                    if (++NPC.frameCounter >= 6)
                    {
                        NPC.frameCounter = 0;
                        AniFrameY++;
                        if (AniFrameY > 10)
                            AniFrameY = 7;
                    }
                    break;
                case ActionState.Slam:
                    if (++NPC.frameCounter >= 6)
                    {
                        NPC.frameCounter = 0;
                        AniFrameY++;
                        if (AniFrameY > 13)
                        {
                            AIState = ActionState.Typing;
                            AniFrameY = 7;
                        }
                    }
                    break;
                case ActionState.SlamLeft:
                    if (++NPC.frameCounter >= 6)
                    {
                        NPC.frameCounter = 0;
                        AniFrameY++;
                        if (AniFrameY > 17)
                        {
                            AIState = ActionState.Typing;
                            AniFrameY = 7;
                            AniFrameX = 0;
                        }
                    }
                    break;
                case ActionState.SlamRight:
                    if (++NPC.frameCounter >= 6)
                    {
                        NPC.frameCounter = 0;
                        AniFrameY++;
                        if (AniFrameY > 16)
                        {
                            AIState = ActionState.Typing;
                            AniFrameY = 7;
                        }
                    }
                    break;
                case ActionState.Kick:
                    if (++NPC.frameCounter >= 6)
                    {
                        NPC.frameCounter = 0;
                        AniFrameY++;
                        if (AniFrameY > 2)
                        {
                            AIState = ActionState.Typing;
                            AniFrameY = 7;
                        }
                    }
                    break;
                case ActionState.Headbutt:
                    if (++NPC.frameCounter >= 6)
                    {
                        NPC.frameCounter = 0;
                        AniFrameY++;
                        if (AniFrameY > 11)
                        {
                            AIState = ActionState.Typing;
                            AniFrameX = 0;
                            AniFrameY = 7;
                        }
                    }
                    break;
                case ActionState.Defeat:
                    if (++NPC.frameCounter >= 6)
                    {
                        NPC.frameCounter = 0;
                        AniFrameY++;
                        if (AniFrameY > 14)
                        {
                            AniFrameY = 0;
                            AniFrameX = 0;
                            AIState = ActionState.WalkAway;
                        }
                    }
                    break;
                case ActionState.WalkAway:
                    if (++NPC.frameCounter >= 6)
                    {
                        NPC.frameCounter = 0;
                        AniFrameY++;
                        if (AniFrameY > 7)
                            AniFrameY = 0;
                    }
                    break;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D Anims = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Anims").Value;
            Texture2D StepAni = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Step").Value;
            Texture2D WalkAni = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Walk").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (AIState is ActionState.WalkAway)
            {
                int Height = WalkAni.Height / 8;
                int y = Height * AniFrameY;
                Rectangle rect = new(0, y, WalkAni.Width, Height);
                Vector2 origin = new(WalkAni.Width / 2f, Height / 2f);
                spriteBatch.Draw(WalkAni, NPC.Center - screenPos + new Vector2(12, -4), new Rectangle?(rect), NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, effects, 0);
            }
            else if (AIState is ActionState.Kick)
            {
                int Height = StepAni.Height / 3;
                int y = Height * AniFrameY;
                Rectangle rect = new(0, y, StepAni.Width, Height);
                Vector2 origin = new(StepAni.Width / 2f, Height / 2f);
                spriteBatch.Draw(StepAni, NPC.Center - screenPos + new Vector2(1, -10), new Rectangle?(rect), NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, effects, 0);
            }
            else if (AIState != ActionState.Idle)
            {
                int Height = Anims.Height / 18;
                int y = Height * AniFrameY;
                int Width = Anims.Width / 2;
                int x = Width * AniFrameX;
                Rectangle rect = new(x, y, Width, Height);
                Vector2 origin = new(Width / 2f, Height / 2f);
                spriteBatch.Draw(Anims, NPC.Center - screenPos + new Vector2(2, -4), new Rectangle?(rect), NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, effects, 0);
            }
            else
                spriteBatch.Draw(texture, NPC.Center - screenPos + new Vector2(1, 1), NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            return false;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanHitNPC(NPC target) => false;
    }
}