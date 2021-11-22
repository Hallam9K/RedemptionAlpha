using Microsoft.Xna.Framework;
using System;
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

        readonly double dist = 1500; //Radius of orbit
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
            Lighting.AddLight(NPC.Center, (255 - NPC.alpha) * 1f / 255f, (255 - NPC.alpha) * 1f / 255f, (255 - NPC.alpha) * 1f / 255f);
            double deg = NPC.ai[1]; //Degrees of orbit
            double rad = deg * (Math.PI / 180); //Math nonsense
            NPC host = Main.npc[(int)NPC.ai[0]];
            NPC.position.X = host.Center.X - (int)(Math.Cos(rad) * dist) - NPC.width / 2;
            NPC.position.Y = host.Center.Y - (int)(Math.Sin(rad) * dist) - NPC.height / 2;
            NPC.ai[1] += 0.8f; //Orbit Speed
            if (host.life <= 0 || !host.active || host.type != ModContent.NPCType<Wielder>())
            {
                NPC.active = false;
                NPC.life = 0;
                NPC.checkDead();
                NPC.HitEffect();
            }

            float num = 8f;
            Vector2 vector = new(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
            float hostX = host.position.X + (host.width / 2);
            float hostY = host.position.Y + (host.height / 2);
            hostX = (int)(hostX / 8f) * 8;
            hostY = (int)(hostY / 8f) * 8;
            vector.X = (int)(vector.X / 8f) * 8;
            vector.Y = (int)(vector.Y / 8f) * 8;
            hostX -= vector.X;
            hostY -= vector.Y;
            float rootXY = (float)Math.Sqrt(hostX * hostX + hostY * hostY);
            if (rootXY == 0f)
            {
                hostX = NPC.velocity.X;
                hostY = NPC.velocity.Y;
            }
            else
            {
                rootXY = num / rootXY;
                hostX *= rootXY;
                hostY *= rootXY;
            }
            NPC.rotation = (float)Math.Atan2(hostY, hostX) + 3.14f + MathHelper.ToRadians(-90f);
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            NPC host = Main.npc[(int)NPC.ai[0]];
            return host.ai[0] != 11;
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