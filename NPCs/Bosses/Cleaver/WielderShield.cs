using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Redemption.NPCs.Bosses.Cleaver
{
    [AutoloadBossHead]
    public class WielderShield : ModNPC
    {
        public override string Texture => "Redemption/NPCs/Bosses/Cleaver/OmegaPlasmaBall";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Barrier Orbital");
            Main.npcFrameCount[NPC.type] = 4;
            
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override bool CheckActive()
        {
            if (NPC.CountNPCS(ModContent.NPCType<Wielder>()) >= 1)
                return false;
            return true;
        }
        public override void SetDefaults()
        {
            NPC.width = 48;
            NPC.height = 48;
            NPC.damage = 120;
            NPC.defense = 0;
            NPC.lifeMax = 1000;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.lavaImmune = true;
            NPC.dontTakeDamage = true;
        }

        public override void AI()
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += 50;
                if (NPC.frame.Y > 150)
                    NPC.frame.Y = 0;
            }
            Lighting.AddLight(NPC.Center, NPC.Opacity, NPC.Opacity, NPC.Opacity);
            NPC host = Main.npc[(int)NPC.ai[0]];
            NPC.localAI[0] += 0.02f;
            NPC.Center = host.Center + Vector2.One.RotatedBy(MathHelper.ToRadians(NPC.ai[1]) + NPC.localAI[0]) * 1000;
            NPC.rotation = (host.Center - NPC.Center).ToRotation();

            if (host.life <= 0 || !host.active || host.type != ModContent.NPCType<Wielder>())
            {
                NPC.active = false;
                NPC.life = 0;
                NPC.checkDead();
                NPC.HitEffect();
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            NPC host = Main.npc[(int)NPC.ai[0]];
            return host.ai[0] != 6;
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.LifeDrain, 0f, 0f, 100, default, 2f);
                    dust.velocity = -NPC.DirectionTo(dust.position) * 5f;
                }
            }
        }
    }
}