using Microsoft.Xna.Framework;
using Redemption.Biomes;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Globals.NPCs;
using Redemption.Items.Armor.Vanity.Intruder;
using Redemption.Items.Materials.HM;
using Redemption.Items.Placeable.Banners;
using Redemption.Items.Usable.Potions;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs.Wasteland
{
    internal class BloatedDiggerHead : WormHead
    {
        public override int BodyType => ModContent.NPCType<BloatedDiggerBody>();
        public override int TailType => ModContent.NPCType<BloatedDiggerTail>();
        public override void SetStaticDefaults()
        {
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Infected);

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Redemption/Textures/Bestiary/BloatedDigger_Bestiary",
                Position = new Vector2(20f, 24f),
                PortraitPositionXOverride = -12f,
                PortraitPositionYOverride = 12f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.aiStyle = -1;

            NPC.lifeMax = 400;
            NPC.damage = 50;
            NPC.defense = 35;
            NPC.width = 30;
            NPC.height = 34;
            NPC.value = 100f;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<WastelandPurityBiome>().Type };
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<BloatedDiggerBanner>();
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.BloatedDigger"))
            });
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.BeatAnyMechBoss(), ModContent.ItemType<Xenomite>(), 2, 3, 6));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ToxicBile>(), 2, 3, 6));
            npcLoot.Add(ItemDropRule.OneFromOptions(50, ModContent.ItemType<IntruderMask>(), ModContent.ItemType<IntruderArmour>(), ModContent.ItemType<IntruderPants>()));
            npcLoot.Add(ItemDropRule.Food(ModContent.ItemType<StarliteDonut>(), 150));
            var dropRules = Main.ItemDropsDB.GetRulesForNPCID(NPCID.DiggerHead, false);
            foreach (var dropRule in dropRules)
                npcLoot.Add(dropRule);
        }
        public override void Init()
        {
            MinSegmentLength = 8;
            MaxSegmentLength = 14;

            CommonWormInit(this);
        }

        internal static void CommonWormInit(Worm worm)
        {
            worm.MoveSpeed = 7f;
            worm.Acceleration = 0.08f;
        }
        private int attackCounter;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(attackCounter);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            attackCounter = reader.ReadInt32();
        }
        public override void AI()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (attackCounter > 0)
                    attackCounter--;

                Player target = Main.player[NPC.target];
                if (attackCounter <= 0 && NPC.DistanceSQ(target.Center) < 400 * 400)
                {
                    SoundEngine.PlaySound(SoundID.Zombie24, NPC.position);
                    Vector2 direction = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
                    direction = direction.RotatedByRandom(MathHelper.ToRadians(10));
                    NPC.velocity = direction * 10;

                    attackCounter = Main.rand.Next(180, 501);
                    NPC.netUpdate = true;
                }
            }
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 20; i++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenBlood, Scale: 1.2f);
                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/BloatedDiggerHeadGore").Type, 1);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (Main.rand.NextBool(2) || Main.expertMode)
                target.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), Main.rand.Next(300, 1200));
        }
    }

    internal class BloatedDiggerBody : WormBody
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerBody);
            NPC.aiStyle = -1;

            NPC.lifeMax = 400;
            NPC.damage = 50;
            NPC.defense = 35;
            NPC.width = 18;
            NPC.height = 28;
            NPC.value = 100f;
        }
        public override void Init()
        {
            BloatedDiggerHead.CommonWormInit(this);
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 20; i++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenBlood, Scale: 1.2f);
                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/BloatedDiggerBodyGore").Type, 1);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (Main.rand.NextBool(2) || Main.expertMode)
                target.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), Main.rand.Next(300, 1200));
        }
    }

    internal class BloatedDiggerTail : WormTail
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerTail);
            NPC.aiStyle = -1;

            NPC.lifeMax = 400;
            NPC.damage = 50;
            NPC.defense = 35;
            NPC.width = 22;
            NPC.height = 36;
            NPC.value = 100f;
        }
        public override void Init()
        {
            BloatedDiggerHead.CommonWormInit(this);
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 20; i++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenBlood, Scale: 1.2f);
                Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/BloatedDiggerTailGore").Type, 1);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (Main.rand.NextBool(2) || Main.expertMode)
                target.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), Main.rand.Next(300, 1200));
        }
    }
}