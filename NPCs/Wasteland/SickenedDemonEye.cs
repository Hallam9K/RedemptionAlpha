using Redemption.Biomes;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Banners;
using Redemption.Items.Usable.Potions;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Wasteland
{
    public class SickenedDemonEye : ModNPC
    {
        public ref float AITimer => ref NPC.ai[0];
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2;
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Poisoned,
                    ModContent.BuffType<BileDebuff>(),
                    ModContent.BuffType<GreenRashesDebuff>(),
                    ModContent.BuffType<GlowingPustulesDebuff>(),
                    ModContent.BuffType<FleshCrystalsDebuff>()
                }
            });

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DemonEye);
            NPC.damage = 50;
            NPC.defense = 16;
            NPC.lifeMax = 320;
            NPC.value = 200f;
            NPC.knockBackResist = 0.3f;
            NPC.aiStyle = 2;
            AIType = NPCID.DemonEye;
            AnimationType = NPCID.DemonEye;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<WastelandPurityBiome>().Type };
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<SickenedDemonEyeBanner>();
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement(
                    "Is it crying? No, that's just foul smelling pus leaking out.")
            });
        }
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (Main.rand.NextBool(2) || Main.expertMode)
                target.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), Main.rand.Next(400, 900));
        }
        public override bool PreAI()
        {
            Player player = Main.player[NPC.target];
            if (Main.rand.NextBool(300) && NPC.DistanceSQ(player.Center) < 400 * 400)
            {
                NPC.ai[2] = 1;
                NPC.netUpdate = true;
            }
            if (NPC.ai[2] != 0)
            {
                NPC.velocity *= 0.96f;
                NPC.rotation = NPC.DirectionTo(player.Center).ToRotation();
                if (AITimer++ >= 80 && NPC.velocity.Length() < 1)
                {
                    NPC.velocity = NPC.DirectionTo(player.Center) * 10;
                    AITimer = 0;
                    NPC.ai[2] = 0;
                    NPC.netUpdate = true;
                }
                return false;
            }
            return true;
        }
        public override void PostAI()
        {
            Lighting.AddLight(NPC.Center, NPC.Opacity * 0.1f, NPC.Opacity * 0.2f, NPC.Opacity * 0.1f);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<XenomiteShard>(), 2, 3, 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ToxicBile>(), 4, 1, 3));
            npcLoot.Add(ItemDropRule.OneFromOptions(50, ModContent.ItemType<IntruderMask>(), ModContent.ItemType<IntruderArmour>(), ModContent.ItemType<IntruderPants>()));
            npcLoot.Add(ItemDropRule.Food(ModContent.ItemType<StarliteDonut>(), 150));
            var dropRules = Main.ItemDropsDB.GetRulesForNPCID(NPCID.DemonEye, false);
            foreach (var dropRule in dropRules)
            {
                npcLoot.Add(dropRule);
            }
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 25; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood);

                for (int i = 0; i < 2; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/SickenedDemonEyeGore" + (i + 1)).Type, 1);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood);
        }
    }
}