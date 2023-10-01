using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Redemption.Dusts;
using Redemption.Globals;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Redemption.Biomes;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria.GameContent;
using Redemption.Base;
using Terraria.Audio;
using Redemption.Projectiles.Hostile;
using Redemption.Globals.NPC;

namespace Redemption.NPCs.Bosses.SeedOfInfection
{
    public class SeedGrowth : ModNPC
    {
        private static Asset<Texture2D> chainTexture;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            chainTexture = ModContent.Request<Texture2D>(Texture + "_Chain");
        }
        public override void Unload()
        {
            chainTexture = null;
        }
        public static int BodyType() => ModContent.NPCType<SoI>();

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Hive Growth");
            Main.npcFrameCount[NPC.type] = 4;
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.NoBlood);
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Infected);

            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            ElementID.NPCPoison[Type] = true;
            ElementID.NPCWater[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 40;
            NPC.damage = 26;
            NPC.defense = 0;
            NPC.knockBackResist = 0.1f;
            NPC.width = 26;
            NPC.height = 26;
            NPC.value = Item.buyPrice(0, 0, 0, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.alpha = 225;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath19;
            NPC.behindTiles = true;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<WastelandPurityBiome>().Type };
        }
        public override Color? GetAlpha(Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                return NPC.GetBestiaryEntryColor();
            }
            return null;
        }

        public override void OnKill()
        {
            if (Main.rand.NextBool(8))
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ItemID.Heart);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<SludgeDust>(), Scale: 2);
                    Main.dust[dustIndex].velocity *= 2f;
                }
                NPC host = Main.npc[(int)NPC.ai[0]];
                int steps = (int)NPC.Distance(host.Center) / 8;
                for (int i = 0; i < steps; i++)
                {
                    for (int j = 0; j < 10; j++)
                        Dust.NewDust(Vector2.Lerp(NPC.Center, host.Center, (float)i / steps) - new Vector2(4, 4), 8, 8, ModContent.DustType<SludgeDust>(), Scale: 2);
                }
            }
        }

        public override void AI()
        {
            NPC host = Main.npc[(int)NPC.ai[0]];
            if (Main.netMode != NetmodeID.MultiplayerClient && (!host.active || host.type != BodyType() || NPC.DistanceSQ(host.Center) > 800 * 800))
                NPC.StrikeInstantKill();
            if (NPC.DespawnHandler(0, 10))
                return;

            if (NPC.ai[2]++ >= 600)
            {
                NPC.Move(host.Center, (NPC.ai[2] - 600) / 10, 10);
                if (NPC.DistanceSQ(host.Center) <= 20 * 20)
                {
                    host.HealEffect(NPC.life);
                    host.life += NPC.life;
                    if (host.life > host.lifeMax)
                        host.life = host.lifeMax;
                    SoundEngine.PlaySound(SoundID.Item2, NPC.position);
                    BaseAI.DamageNPC(NPC, NPC.lifeMax + 10, 0, host, false, true);
                    for (int i = 0; i < 24; i++)
                    {
                        NPC.Shoot(host.Center, ModContent.ProjectileType<BloatedClinger_Gas>(), host.damage, RedeHelper.PolarVector(-Main.rand.Next(10, 39), (host.Center - Main.player[host.target].Center).ToRotation() + Main.rand.NextFloat(-.1f, .1f)), SoundID.NPCDeath13);
                    }
                }
                return;
            }
            NPC.Move(Vector2.Zero, 14, 30, true);
            if (NPC.DistanceSQ(host.Center) > NPC.ai[1] * NPC.ai[1])
                NPC.velocity -= host.Center.DirectionTo(NPC.position);

            NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -20, 20);
            NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -20, 20);

            if (NPC.alpha > 0)
                NPC.alpha -= 20;
            OverlapCheck();
        }
        private void OverlapCheck()
        {
            float overlapVelocity = 0.2f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC other = Main.npc[i];

                if (i != NPC.whoAmI && other.active && other.type == Type && Math.Abs(NPC.position.X - other.position.X) + Math.Abs(NPC.position.Y - other.position.Y) < NPC.width)
                {
                    if (NPC.position.X < other.position.X)
                        NPC.velocity.X -= overlapVelocity;
                    else
                        NPC.velocity.X += overlapVelocity;

                    if (NPC.position.Y < other.position.Y)
                        NPC.velocity.Y -= overlapVelocity;
                    else
                        NPC.velocity.Y += overlapVelocity;
                }
            }
        }
        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter >= 10)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 3 * frameHeight)
                    NPC.frame.Y = 0;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                return true;
            NPC host = Main.npc[(int)NPC.ai[0]];
            Vector2 anchorPos = host.Center;
            Vector2 HeadPos = NPC.Center;
            Rectangle sourceRectangle = new(0, 0, chainTexture.Value.Width, chainTexture.Value.Height);
            Vector2 origin = new(chainTexture.Value.Width * 0.5f, chainTexture.Value.Height * 0.5f);
            float num1 = chainTexture.Value.Height;
            Vector2 vector2_4 = anchorPos - HeadPos;
            float rotation = (float)Math.Atan2(vector2_4.Y, vector2_4.X) - 1.57f;
            bool flag = true;
            if (float.IsNaN(HeadPos.X) && float.IsNaN(HeadPos.Y))
                flag = false;
            if (float.IsNaN(vector2_4.X) && float.IsNaN(vector2_4.Y))
                flag = false;
            while (flag)
            {
                if (vector2_4.Length() < num1 + 1.0)
                    flag = false;
                else
                {
                    Vector2 vector2_1 = vector2_4;
                    vector2_1.Normalize();
                    HeadPos += vector2_1 * num1;
                    vector2_4 = anchorPos - HeadPos;
                    Color color = Lighting.GetColor((int)HeadPos.X / 16, (int)(HeadPos.Y / 16));
                    Main.EntitySpriteDraw(chainTexture.Value, HeadPos - Main.screenPosition, new Rectangle?(sourceRectangle), NPC.GetAlpha(color), rotation, origin, 1, SpriteEffects.None, 0);
                }
            }
            Main.EntitySpriteDraw(TextureAssets.Npc[Type].Value, NPC.Center - Main.screenPosition, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = BodyType();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement("Absolute BEBE")
            });
        }
    }
}