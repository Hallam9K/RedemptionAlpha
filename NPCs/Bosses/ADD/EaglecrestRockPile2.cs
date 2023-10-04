using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Globals.NPC;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.ADD
{
    public class EaglecrestRockPile2 : ModNPC
    {
        public override string Texture => "Redemption/NPCs/Minibosses/EaglecrestGolem/EaglecrestRockPile";
        public static int BodyType() => ModContent.NPCType<EaglecrestGolem2>();

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Eaglecrest Rock Pile");
            Main.npcFrameCount[NPC.type] = 8;

            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 4000;
            NPC.damage = 98;
            NPC.defense = 38;
            NPC.knockBackResist = 0.01f;
            NPC.aiStyle = -1;
            NPC.width = 42;
            NPC.height = 54;
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath43;
            NPC.lavaImmune = true;
            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Earth] *= .75f;
        }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];
            NPC.LookAtEntity(player);

            int host = NPC.FindFirstNPC(BodyType());
            if (host > -1 && Main.npc[host].ai[0] == 5)
                BaseAI.DamageNPC(NPC, 99999, 0, player, false, true);

            if (AITimer == 0)
            {
                TimerRand = Main.rand.NextFloat(-0.4f, 0.4f);
                AITimer = 1;
            }
            NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20);
            NPCHelper.HorizontallyMove(NPC, player.Center, 0.14f, 7f + TimerRand, 26, 30, NPC.Center.Y > player.Center.Y, player);
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        public override void FindFrame(int frameHeight)
        {
            if (NPC.collideY || NPC.velocity.Y == 0)
            {
                NPC.rotation.SlowRotation(0, (float)Math.PI / 20);

                NPC.frameCounter += NPC.velocity.X * 0.5f;
                if (NPC.frameCounter >= 3 || NPC.frameCounter <= -3)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 7 * frameHeight)
                        NPC.frame.Y = 0;
                }
            }
            else
            {
                NPC.rotation += NPC.velocity.X * 0.07f;
                NPC.frame.Y = 0;
            }
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 25; i++)
                {
                    int dustIndex2 = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Stone);
                    Main.dust[dustIndex2].velocity *= 4f;
                }
                for (int i = 0; i < 2; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/EaglecrestRockpileGore" + (i + 1)).Type, 1);
            }
            int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Stone);
            Main.dust[dustIndex].velocity *= 1f;
        }
    }
}


