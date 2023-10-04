using Redemption.Biomes;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Armor.Vanity.Intruder;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Banners;
using Redemption.Items.Usable.Potions;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs.Wasteland
{
    public class RadioactiveJelly : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Radioactive Physalia");
            Main.npcFrameCount[NPC.type] = 7;
            NPCID.Sets.ShimmerTransformToNPC[NPC.type] = NPCID.GreenJellyfish;
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Infected);

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCWater[Type] = true;
            ElementID.NPCPoison[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 86;
            NPC.damage = 75;
            NPC.defense = 38;
            NPC.lifeMax = 450;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath19;
            NPC.value = 2000f;
            NPC.knockBackResist = 0.35f;
            NPC.aiStyle = 18;
            AIType = NPCID.GreenJellyfish;
            AnimationType = NPCID.GreenJellyfish;
            NPC.noGravity = true;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<WastelandPurityBiome>().Type };
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<RadioactiveJellyBanner>();
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.RadioactiveJelly"))
            });
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (Main.rand.NextBool(2) || Main.expertMode)
                target.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), Main.rand.Next(500, 900));
        }
        public override void PostAI()
        {
            Lighting.AddLight(NPC.Center, NPC.Opacity * 0.3f, NPC.Opacity * 0.6f, NPC.Opacity * 0.3f);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.BeatAnyMechBoss(), ModContent.ItemType<XenomiteShard>(), 2, 3, 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ToxicBile>(), 4, 1, 3));
            npcLoot.Add(ItemDropRule.OneFromOptions(50, ModContent.ItemType<IntruderMask>(), ModContent.ItemType<IntruderArmour>(), ModContent.ItemType<IntruderPants>()));
            npcLoot.Add(ItemDropRule.Food(ModContent.ItemType<StarliteDonut>(), 150));
            var dropRules = Main.ItemDropsDB.GetRulesForNPCID(NPCID.GreenJellyfish, false);
            foreach (var dropRule in dropRules)
            {
                npcLoot.Add(dropRule);
            }
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 25; i++)
                {
                    int dustIndex2 = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood, Scale: 2);
                    Main.dust[dustIndex2].velocity *= 2f;
                }
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood);
        }
    }
}
