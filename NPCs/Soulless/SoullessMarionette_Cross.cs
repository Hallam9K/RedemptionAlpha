using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Dusts;
using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.Biomes;
using Terraria.DataStructures;

namespace Redemption.NPCs.Soulless
{
    public class SoullessMarionette_Cross : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Marionette Cross");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true};
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 3712;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.knockBackResist = 0f;
            NPC.width = 52;
            NPC.height = 32;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit48;
            NPC.DeathSound = SoundID.NPCDeath50;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<SoullessBiome>().Type };
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 30; i++)
                {
                    int dustIndex2 = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<VoidFlame>(), Scale: 2);
                    Main.dust[dustIndex2].velocity *= 3f;
                }
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override bool CheckActive()
        {
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
                return true;
            return false;
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest(true);

            NPC doll = Main.npc[(int)NPC.ai[0]];
            if (!doll.active || doll.type != ModContent.NPCType<SoullessMarionette_Doll>())
                NPC.active = false;

            NPC.LookAtEntity(player);
            if (NPC.ai[1]++ % 30 == 0)
            {
                int steps = (int)NPC.Distance(doll.Center) / 32;
                for (int i = 0; i < steps; i++)
                {
                    if (Main.rand.NextBool(8))
                    {
                        Dust dust = Dust.NewDustDirect(Vector2.Lerp(NPC.Center, doll.Center, (float)i / steps), 2, 2, DustID.AncientLight, Scale: 2);
                        dust.velocity *= 0f;
                        dust.noGravity = true;
                    }
                }
            }
            if (NPC.life <= NPC.lifeMax / 2)
                doll.ai[1] = 2;
            if (NPC.DistanceSQ(doll.Center) > 800 * 800)
                doll.velocity = doll.DirectionTo(NPC.position) * 4;
        }
        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter >= 7)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 3 * frameHeight)
                    NPC.frame.Y = 0;
            }
        }
    }
}