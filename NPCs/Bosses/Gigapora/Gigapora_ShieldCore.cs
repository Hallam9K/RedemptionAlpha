using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Redemption.Dusts;
using Redemption.Globals;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Redemption.Biomes;
using Terraria.DataStructures;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Redemption.Base;

namespace Redemption.NPCs.Bosses.Gigapora
{
    [AutoloadBossHead]
    public class Gigapora_ShieldCore : ModNPC
    {
        public static int BodyType() => ModContent.NPCType<Gigapora>();

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shield Core");
            Main.npcFrameCount[NPC.type] = 19;
            NPCDebuffImmunityData debuffData = new()
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Confused,
                    BuffID.Poisoned,
                    BuffID.Venom,
                    ModContent.BuffType<InfestedDebuff>(),
                    ModContent.BuffType<NecroticGougeDebuff>(),
                    ModContent.BuffType<ViralityDebuff>(),
                    ModContent.BuffType<DirtyWoundDebuff>()
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);

            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 20000;
            NPC.damage = 60;
            NPC.defense = 25;
            NPC.knockBackResist = 0;
            NPC.width = 76;
            NPC.height = 44;
            NPC.value = 0;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<LidenBiomeOmega>().Type, ModContent.GetInstance<LidenBiome>().Type };
        }

        public override void OnKill()
        {
            Item.NewItem(NPC.getRect(), ItemID.Heart);
            Item.NewItem(NPC.getRect(), ItemID.Heart);
            NPC seg = Main.npc[(int)NPC.ai[0]];
            if (seg.active && seg.type == ModContent.NPCType<Gigapora_BodySegment>())
            {
                seg.ai[0] = 2;
                Main.npc[seg.whoAmI - 1].ai[0] = 2;
                Main.npc[seg.whoAmI + 1].ai[0] = 2;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.LifeDrain, Scale: 2);
                    Main.dust[dustIndex].velocity *= 2f;
                }
                for (int i = 0; i < 6; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Electric);
                    Main.dust[dustIndex].velocity *= 2f;
                }
            }
        }

        public override void AI()
        {
            NPC seg = Main.npc[(int)NPC.ai[0]];
            if (!seg.active || seg.type != ModContent.NPCType<Gigapora_BodySegment>())
                NPC.active = false;
            DespawnHandler();
            if (NPC.ai[1]++ >= 85)
                NPC.Move(Vector2.Zero, 8, 60, true);
            else
                NPC.velocity *= 0.99f;

            if (NPC.ai[3] > 0)
            {
                NPC.ai[3]++;
                NPC.velocity *= 0.9f;
                if (NPC.ai[3] >= 180)
                    NPC.ai[3] = 0;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.rotation = NPC.velocity.X * 0.07f;
            if (++NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 18 * frameHeight)
                    NPC.frame.Y = 10 * frameHeight;
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = BodyType();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement("Absolute BEBE")
            });
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D glowMask = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Glow").Value;
            Texture2D glowRadius = ModContent.Request<Texture2D>("Redemption/Textures/WhiteGlow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (!NPC.IsABestiaryIconDummy)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                float glowOpacity = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 0.3f, 0.5f, 0.3f);
                float glowSize = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1.5f, 1f, 1.5f);
                float glowSize2 = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1f, 1.5f, 1f);
                Vector2 glowOrigin = new(glowRadius.Width / 2f, glowRadius.Height / 2f);
                spriteBatch.Draw(glowRadius, NPC.Center - screenPos, null, Color.Red * glowOpacity, 0, glowOrigin, glowSize, effects, 0);
                spriteBatch.Draw(glowRadius, NPC.Center - screenPos, null, Color.Red * glowOpacity * 0.7f, 0, glowOrigin, glowSize2 * 1.5f, effects, 0);

                spriteBatch.Draw(ModContent.Request<Texture2D>("Redemption/Textures/FadeTelegraph").Value, NPC.Center - screenPos, new Rectangle(0, 0, 64, 128), Color.Red * glowOpacity, (Main.npc[(int)NPC.ai[0]].Center - NPC.Center).ToRotation(), new Vector2(0, 64), new Vector2(NPC.Distance(Main.npc[(int)NPC.ai[0]].Center) / 150, NPC.width / 128f), SpriteEffects.None, 0f);
                spriteBatch.Draw(ModContent.Request<Texture2D>("Redemption/Textures/FadeTelegraphCap").Value, NPC.Center - screenPos, new Rectangle(0, 0, 64, 128), Color.Red * glowOpacity, (NPC.Center - Main.npc[(int)NPC.ai[0]].Center).ToRotation(), new Vector2(0, 64), new Vector2(NPC.width / 128f, NPC.width / 128f), SpriteEffects.None, 0f);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }
            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(glowMask, NPC.Center - screenPos, NPC.frame, RedeColor.RedPulse, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            return false;
        }

        private void DespawnHandler()
        {
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    NPC.velocity.Y = -10;
                    if (NPC.timeLeft > 10)
                        NPC.timeLeft = 10;
                    return;
                }
            }
        }
    }
}