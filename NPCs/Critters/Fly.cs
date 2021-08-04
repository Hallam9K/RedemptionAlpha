using Microsoft.Xna.Framework;
using Redemption.Items.Critters;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Critters
{
    public class Fly : ModNPC
    {
        private enum ActionState
        {
            Begin,
            Flying,
            Landed
        }
        public ref float AIState => ref NPC.ai[0];
        public ref float AITimer => ref NPC.ai[1];
        public ref float TimerRand => ref NPC.ai[2];
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            NPCID.Sets.CountsAsCritter[Type] = true;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 8;
            NPC.height = 8;
            NPC.lifeMax = 1;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.npcSlots = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.catchItem = (short)ModContent.ItemType<FlyBait>();
        }
        NPC target;
        public Vector2 moveTo;
        public override void AI()
        {
            NPC.TargetClosest();
            if (Math.Abs(NPC.velocity.X) > 0.2)
            {
                NPC.spriteDirection = -NPC.direction;
            }
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.Pi;
            if (NPC.collideX && NPC.velocity.X != NPC.oldVelocity.X)
            {
                NPC.velocity.X = -NPC.oldVelocity.X;
            }
            if (NPC.collideY && NPC.velocity.Y != NPC.oldVelocity.Y)
            {
                NPC.velocity.Y = -NPC.oldVelocity.Y;
            }
            switch (AIState)
            {
                case (float)ActionState.Begin:
                    TimerRand = Main.rand.Next(240, 600);
                    AIState = (float)ActionState.Flying;
                    NPC.velocity = RedeHelper.PolarVector(10, Main.rand.NextFloat(0, MathHelper.TwoPi));
                    break;
                case (float)ActionState.Flying:
                    NPC.noGravity = true;
                    NPC.rotation = NPC.velocity.ToRotation() + MathHelper.Pi;
                    if (NPC.velocity.Length() < 4)
                    {
                        NPC.velocity = RedeHelper.PolarVector(10, Main.rand.NextFloat(0, MathHelper.TwoPi));
                    }
                    NPC.velocity = NPC.velocity.RotatedBy(Main.rand.NextFloat(-1f, 1f));
                    AITimer++;
                    if (AITimer >= TimerRand)
                    {
                        AITimer = 0;
                        TimerRand = Main.rand.Next(240, 320);
                        AIState = (float)ActionState.Landed;
                    }
                    break;
                case (float)ActionState.Landed:
                    AITimer++;
                    NPC.rotation = 0;
                    NPC.noGravity = false;
                    if (BaseAI.HitTileOnSide(NPC, 3, false))
                    {
                        NPC.velocity *= 0;
                    }
                    else
                    {
                        NPC.velocity.Y++;
                        NPC.velocity.X *= 0.98f;
                    }
                    if (AITimer >= TimerRand)
                    {
                        NPC.velocity.Y -= 10;
                        NPC.velocity = RedeHelper.PolarVector(10, Main.rand.NextFloat(0, MathHelper.TwoPi));
                        AITimer = 0;
                        TimerRand = Main.rand.Next(240, 600);
                        AIState = (float)ActionState.Flying;
                    }
                    if (RedeHelper.ClosestNPCAny(ref target, 100, NPC.Center) && target.life > 5 && target.type != ModContent.NPCType<Fly>())
                    {
                        NPC.velocity.Y -= 10;
                        NPC.velocity = RedeHelper.PolarVector(10, Main.rand.NextFloat(0, MathHelper.TwoPi));
                        AITimer = 0;
                        TimerRand = Main.rand.Next(240, 600);
                        AIState = (float)ActionState.Flying;
                    }
                    if (NPC.Distance(RedeHelper.GetNearestAlivePlayerVector(NPC)) <= 100)
                    {
                        NPC.velocity.Y -= 10;
                        NPC.velocity = RedeHelper.PolarVector(10, Main.rand.NextFloat(0, MathHelper.TwoPi));
                        AITimer = 0;
                        TimerRand = Main.rand.Next(240, 600);
                        AIState = (float)ActionState.Flying;
                    }
                    break;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            if (AIState == (float)ActionState.Landed && NPC.velocity.Y == 0)
            {
                NPC.frame.Y = 0;
            }
            else
            {
                NPC.frameCounter++;
                if (NPC.frameCounter >= 4)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 3 * frameHeight)
                    {
                        NPC.frame.Y = 0;
                    }
                }
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

                new FlavorTextBestiaryInfoElement("A pesky annoyance that only exists to ruin your day.")
            });
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
    }
}