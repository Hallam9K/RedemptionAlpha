using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Biomes;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Globals.NPC;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs.Lab.Blisterface
{
    public class BlisteredFish : ModNPC
    {
        public static int BodyType() => ModContent.NPCType<Blisterface>();
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Infected);
            if (NPC.type == ModContent.NPCType<BlisteredFish>())
            {
                NPCID.Sets.BossBestiaryPriority.Add(Type);
                NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
                {
                    Velocity = 1
                };
                NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            }
        }
        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 28;
            NPC.friendly = false;
            NPC.damage = 60;
            NPC.defense = 0;
            NPC.lifeMax = 750;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 0f;
            NPC.noGravity = true;
            NPC.knockBackResist = 0.1f;
            NPC.aiStyle = 16;
            AIType = NPCID.Piranha;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<LabBiome>().Type };
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 10; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood, Scale: 1.5f);
            }
        }
        public override void PostAI()
        {
            NPC.LookByVelocity();
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = BodyType();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.BlisteredFish"))
            });
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 10)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 5 * frameHeight)
                    NPC.frame.Y = 0;
            }
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (Main.rand.NextBool(2) || Main.expertMode)
                target.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), Main.rand.Next(200, 600));
        }
    }
    public class BlisteredFish2 : BlisteredFish
    {
        public override string Texture => "Redemption/NPCs/Lab/Blisterface/BlisteredFish";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Blistered Fish");
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        private int Timer;
        private bool GlowActive;
        private int GlowTimer;
        public override void PostAI()
        {
            NPC.LookByVelocity();
            if (GlowActive)
            {
                if (GlowTimer++ > 60)
                {
                    GlowActive = false;
                    GlowTimer = 0;
                }
            }

            if (Timer++ == 80)
                GlowActive = true;
            if (Timer == 120)
            {
                NPC.wet = false;
                NPC.noTileCollide = true;
                NPC.velocity.X = 0;
                NPC.velocity.Y = -15;
            }
            if (Timer >= 180)
            {
                NPC.velocity.Y += 0.15f;
                Point water = NPC.Center.ToTileCoordinates();
                if (Main.tile[water.X, water.Y].LiquidType == LiquidID.Water && Main.tile[water.X, water.Y].LiquidAmount > 0)
                {
                    NPC.wet = true;
                    NPC.noTileCollide = false;
                    Timer = 0;
                }
            }
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Color colour = Color.Lerp(Color.White, Color.White, 1f / GlowTimer * 10f) * (1f / GlowTimer * 10f);
            if (GlowActive)
                spriteBatch.Draw(glow, NPC.Center - screenPos + new Vector2(0, 4), NPC.frame, colour, NPC.rotation, NPC.frame.Size() / 2, 1f, effects, 0);
        }
    }
}