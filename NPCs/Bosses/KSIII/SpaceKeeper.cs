using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using Redemption.Globals.NPCs;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.KSIII
{
    public class SpaceKeeper : ModNPC
    {
        public static int BodyType() => NPCType<KS3>();

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

                new FlavorTextBestiaryInfoElement(Mod.GetLocalization("FlavorTextBestiary.SpaceKeeper").Value)
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
            if (NPC.ai[2] > 180 && host.life < host.lifeMax)
            {
                NPC.ai[3] = 1;
                Dust dust = Dust.NewDustDirect(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Frost, 0, 0, 100, new Color(), 2f);
                dust.velocity = -host.DirectionTo(dust.position) * 20;
                dust.noGravity = true;
                NPC.netUpdate = true;
            }
            if (host.ai[0] == 13 || host.life >= host.lifeMax)
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
                        NPC.MoveToVector2(Main.npc[(int)NPC.ai[0]].Center + new Vector2(-250, -250), 11);
                        break;
                    case 1:
                        NPC.MoveToVector2(Main.npc[(int)NPC.ai[0]].Center + new Vector2(250, -250), 11);
                        break;
                    case 2:
                        NPC.MoveToVector2(Main.npc[(int)NPC.ai[0]].Center + new Vector2(-250, 250), 11);
                        break;
                    case 3:
                        NPC.MoveToVector2(Main.npc[(int)NPC.ai[0]].Center + new Vector2(250, 250), 11);
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
                        NPC.frame.Y = 4 * frameHeight;
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
        private Asset<Texture2D> glowMask;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> texture = TextureAssets.Npc[Type];
            glowMask ??= Request<Texture2D>(Texture + "_Glow");
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture.Value, NPC.Center - screenPos, NPC.frame, NPC.ColorTintedAndOpacity(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(glowMask.Value, NPC.Center - screenPos, NPC.frame, NPC.ColorTintedAndOpacity(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }
    }
}