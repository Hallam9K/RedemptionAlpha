using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.Items.Critters;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Critters
{
    public class AntiJohnSnail : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Anti-John Snail");
            Main.npcFrameCount[Type] = 6;
            NPCID.Sets.CountsAsCritter[Type] = true;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Hide = true,
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 22;
            NPC.height = 16;
            NPC.defense = 0;
            NPC.lifeMax = 5;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 0;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = NPCAIStyleID.Snail;
            AIType = NPCID.Snail;
            AnimationType = NPCID.Snail;
            NPC.catchItem = (short)ModContent.ItemType<AntiJohnSnailItem>();
        }
        public override bool PreAI()
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (!Main.npc[i].active || Main.npc[i].type != ModContent.NPCType<JohnSnail>())
                    continue;

                if (NPC.Hitbox.Intersects(Main.npc[i].Hitbox))
                {
                    SoundEngine.PlaySound(CustomSounds.NukeExplosion with { Volume = 2f }, NPC.position);
                    SoundEngine.PlaySound(CustomSounds.MissileExplosion with { Volume = 2f, Pitch = -.5f }, NPC.position);
                    RedeDraw.SpawnExplosion(NPC.Center + new Vector2(0, 40), Color.Orange, shakeAmount: 100);
                    RedeDraw.SpawnExplosion(NPC.Center + new Vector2(0, 40), Color.Yellow * .6f, scale: 6, shakeAmount: 100);
                    RedeDraw.SpawnExplosion(NPC.Center + new Vector2(0, 40), Color.White * .4f, scale: 8, shakeAmount: 100);
                    if (Main.netMode != NetmodeID.Server)
                    {
                        for (int g = 0; g < 40; g++)
                        {
                            int goreIndex = Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, default, Main.rand.Next(61, 64));
                            Main.gore[goreIndex].scale = 2f;
                            Main.gore[goreIndex].velocity *= Main.rand.NextFloat(6f, 10f);
                        }
                        for (int g = 0; g < 30; g++)
                        {
                            int goreIndex = Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, default, Main.rand.Next(61, 64));
                            Main.gore[goreIndex].scale = 2f;
                            Main.gore[goreIndex].velocity.X *= .3f;
                            Main.gore[goreIndex].velocity.Y -= 3;
                            Main.gore[goreIndex].velocity *= Main.rand.NextFloat(3f, 5f);
                        }
                    }
                    NPC.active = false;
                    Main.npc[i].active = false;
                    RedeHelper.NPCRadiusDamage(800, NPC, 9999, 30);
                    RedeHelper.PlayerRadiusDamage(800, NPC, 9999, 30);
                    break;
                }
            }
            return true;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 4; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.t_Slime, NPC.velocity.X * 0.5f,
                        NPC.velocity.Y * 0.5f);
            }

            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.t_Slime, NPC.velocity.X * 0.5f,
                NPC.velocity.Y * 0.5f);
        }
    }
}