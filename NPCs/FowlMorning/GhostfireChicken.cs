using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Redemption.Dusts;
using Redemption.Globals;
using Terraria.GameContent.Bestiary;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Biomes;
using Terraria.Audio;
using System;

namespace Redemption.NPCs.FowlMorning
{
    public class GhostfireChicken : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ghost-fire Chicken");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Velocity = 1f };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCArcane[Type] = true;
            ElementID.NPCFire[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 28;
            NPC.height = 18;
            NPC.friendly = false;
            NPC.damage = 15;
            NPC.defense = 0;
            NPC.lifeMax = 10;
            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.4f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<FowlMorningBiome>().Type };
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 4; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Smoke,
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

                for (int i = 0; i < 15; i++)
                {
                    int dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<ChickenFeatherDust2>(),
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
                    Main.dust[dust].velocity *= 3f;
                    dust = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<ChickenFeatherDust4>(),
                        NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
                    Main.dust[dust].velocity *= 3f;
                }
            }
            if (Main.rand.NextBool(2))
                Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<ChickenFeatherDust2>(), NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
            if (Main.rand.NextBool(2))
                Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<ChickenFeatherDust4>(), NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastAuraPulse with { Pitch = .2f }, NPC.position);
            for (int i = 0; i < 20; i++)
            {
                int dust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 1f);
                Main.dust[dust2].velocity *= 0;
                Main.dust[dust2].noGravity = true;
                Color dustColor2 = new(217, 84, 155) { A = 0 };
                Main.dust[dust2].color = dustColor2;
                int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 1.2f);
                Main.dust[dust].velocity *= .1f;
                Main.dust[dust].noGravity = true;
                Color dustColor = new(251, 151, 146) { A = 0 };
                Main.dust[dust].color = dustColor;
            }
        }
        private Vector2 flarePos;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.LookByVelocity();
            if (NPC.DespawnHandler(3))
                return;

            NPC.Move(player.Center, 5, 60);

            flarePos = NPC.Center + new Vector2(9 * NPC.spriteDirection, -13);
            int dust2 = Dust.NewDust(flarePos - Vector2.One, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 0.2f);
            Main.dust[dust2].velocity.X *= 0;
            Main.dust[dust2].velocity.Y -= Main.rand.NextFloat(.4f, 1f);
            Main.dust[dust2].noGravity = true;
            Color dustColor2 = new(217, 84, 155) { A = 0 };
            Main.dust[dust2].color = dustColor2;
            int dust = Dust.NewDust(flarePos - Vector2.One, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 0.2f);
            Main.dust[dust].velocity *= .04f;
            Main.dust[dust2].velocity.Y -= Main.rand.NextFloat(.4f, 1f);
            Main.dust[dust].noGravity = true;
            Color dustColor = new(251, 151, 146) { A = 0 };
            Main.dust[dust].color = dustColor;

            FlareTimer += Main.rand.Next(-5, 6);
            FlareTimer = MathHelper.Clamp(FlareTimer, 10, 30);
            FlareScale += Main.rand.NextFloat(-.1f, .1f);
            FlareScale = MathHelper.Clamp(FlareScale, .7f, .9f);

            // If your minion is flying, you want to do this independently of any conditions
            float overlapVelocity = 0.04f;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC other = Main.npc[i];

                if (i != NPC.whoAmI && NPC.type == Type && other.active && Math.Abs(NPC.position.X - other.position.X) + Math.Abs(NPC.position.Y - other.position.Y) < NPC.width)
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
        private float FlareTimer;
        private float FlareScale;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            RedeDraw.DrawEyeFlare(spriteBatch, ref FlareTimer, flarePos - Main.screenPosition, Color.IndianRed, 0, FlareScale);
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.rotation = NPC.velocity.X * 0.05f;
            if (++NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 3 * frameHeight)
                    NPC.frame.Y = 0;
            }
        }
        public static int BodyType() => ModContent.NPCType<Basan>();
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = BodyType();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement(
                    "...")
            });
        }
    }
}
