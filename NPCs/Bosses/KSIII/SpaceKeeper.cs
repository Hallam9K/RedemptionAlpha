using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Base;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Terraria.GameContent;
using Redemption.Globals.NPC;

namespace Redemption.NPCs.Bosses.KSIII
{
    public class SpaceKeeper : ModNPC
    {
        public static int BodyType() => ModContent.NPCType<KS3>();

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 6000;
            NPC.damage = 0;
            NPC.defense = 20;
            NPC.knockBackResist = 0f;
            NPC.width = 44;
            NPC.height = 68;
            NPC.dontTakeDamage = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit42;
            NPC.DeathSound = SoundID.NPCDeath14;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = BodyType();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,

                new FlavorTextBestiaryInfoElement("The 3rd Slayer Unit created by King Slayer III during his million year voyage. Construction began after an alien war which left most of Slayer's androids damaged or broken. This unit specialises in using nanobots to heal other robots.")
            });
        }

        public override void AI()
        {
            NPC host = Main.npc[(int)NPC.ai[0]];
            if (!host.active || host.type != BodyType())
            {
                NPC.active = false;
            }
            NPC.LookAtEntity(host);
            if (NPC.ai[2]++ == 0)
            {
                for (int m = 0; m < 16; m++)
                {
                    int dustID = Dust.NewDust(new Vector2(NPC.Center.X - 1, NPC.Center.Y - 1), 2, 2, DustID.Frost, 0f, 0f, 100, Color.White, 4f);
                    Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(10f, 0f), m / (float)16 * 6.28f);
                    Main.dust[dustID].noLight = false;
                    Main.dust[dustID].noGravity = true;
                }
            }
            if (NPC.ai[2] > 180 && host.life < 10000)
            {
                NPC.ai[3] = 1;
                Dust dust = Dust.NewDustDirect(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Frost, 0, 0, 100, new Color(), 2f);
                dust.velocity = -host.DirectionTo(dust.position) * 20;
                dust.noGravity = true;
                NPC.netUpdate = true;
            }
            if (host.ai[0] == 13)
            {
                NPC.ai[3] = 0;
                NPC.velocity.Y -= 0.5f;
                if (NPC.DistanceSQ(host.Center) > 1500 * 1500)
                {
                    NPC.active = false;
                }
            }
            else
            {
                switch (NPC.ai[1])
                {
                    case 0:
                        NPC.MoveToNPC(Main.npc[(int)NPC.ai[0]], new Vector2(-250, -250), 11, 15);
                        break;
                    case 1:
                        NPC.MoveToNPC(Main.npc[(int)NPC.ai[0]], new Vector2(250, -250), 11, 15);
                        break;
                    case 2:
                        NPC.MoveToNPC(Main.npc[(int)NPC.ai[0]], new Vector2(-250, 250), 11, 15);
                        break;
                    case 3:
                        NPC.MoveToNPC(Main.npc[(int)NPC.ai[0]], new Vector2(250, 250), 11, 15);
                        break;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.ai[3] == 1)
            {
                if (NPC.frame.Y < 4 * frameHeight)
                    NPC.frame.Y = 4 * frameHeight;

                if (NPC.frameCounter > 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 7 * frameHeight)
                        NPC.frame.Y = 4;
                }
                return;
            }
            if (NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 3 * frameHeight)
                    NPC.frame.Y = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D glowMask = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(glowMask, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }
    }
}