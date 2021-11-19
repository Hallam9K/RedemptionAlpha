using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Biomes;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.Dusts;
using Redemption.Globals;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Lab
{
    public class SludgeBlob : ModNPC
    {
        public enum ActionState
        {
            Begin,
            Idle,
            Bounce
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
            DisplayName.SetDefault("Sludge Blob");
            Main.npcFrameCount[NPC.type] = 2;

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Bleeding,
                }
            });

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 16;
            NPC.height = 14;
            NPC.friendly = false;
            NPC.damage = 25;
            NPC.defense = 0;
            NPC.lifeMax = 100;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath19;
            NPC.value = 0f;
            NPC.knockBackResist = 0.6f;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<LabBiome>().Type };
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement("Blob blob blob.")
            });
        }
        public int Xvel;
        public override void AI()
        {
            switch (AIState)
            {
                case ActionState.Begin:
                    TimerRand = Main.rand.Next(10, 30);
                    AIState = ActionState.Idle;
                    break;

                case ActionState.Idle:
                    NPC.LookByVelocity();
                    if (NPC.velocity.Y == 0)
                        NPC.velocity.X *= 0.5f;

                    if (AITimer++ >= TimerRand && (NPC.collideY || NPC.velocity.Y == 0))
                    {
                        Xvel = Main.rand.Next(2, 5);
                        NPC.velocity.X = Xvel * (Main.rand.NextBool(2) ? 1 : -1);
                        NPC.velocity.Y -= Main.rand.Next(4, 7);
                        AITimer = 0;
                        TimerRand = Main.rand.Next(10, 30);
                        AIState = ActionState.Bounce;
                    }
                    break;

                case ActionState.Bounce:
                    NPC.LookByVelocity();
                    if (NPC.collideX)
                        NPC.velocity.X *= -0.7f;
                    if (NPC.collideY || NPC.velocity.Y == 0)
                        AIState = ActionState.Idle;
                    break;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            if (NPC.collideY || NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                if (NPC.frameCounter++ >= 8)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > frameHeight)
                        NPC.frame.Y = 0;
                }
            }
            else
            {
                NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.frame.Y = 0;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 10; i++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<SludgeDust>());
            }
            Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<SludgeDust>());
        }
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (Main.rand.NextBool(2) || Main.expertMode)
                target.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), Main.rand.Next(60, 120));
        }
    }
}