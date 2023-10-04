using Redemption.Biomes;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Globals.NPC;
using Redemption.Items.Accessories.HM;
using Redemption.Items.Armor.Vanity.Intruder;
using Redemption.Items.Materials.HM;
using Redemption.Items.Placeable.Banners;
using Redemption.Items.Usable.Potions;
using Redemption.Items.Weapons.HM.Ranged;
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
    public class BloatedSwarmer : ModNPC
    {
        public ref float AITimer => ref NPC.ai[0];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Bloated Swarmer");
            NPCID.Sets.ShimmerTransformToNPC[NPC.type] = NPCID.FlyingAntlion;
            Main.npcFrameCount[NPC.type] = 8;
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Infected);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCPoison[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 94;
            NPC.height = 54;
            NPC.damage = 70;
            NPC.defense = 10;
            NPC.lifeMax = 570;
            NPC.HitSound = SoundID.NPCHit32;
            NPC.DeathSound = SoundID.NPCDeath35;
            NPC.value = 2000f;
            NPC.knockBackResist = 0.1f;
            NPC.aiStyle = 44;
            AIType = NPCID.FlyingAntlion;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<WastelandDesertBiome>().Type };
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<BloatedSwarmerBanner>();
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 30; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenBlood, Scale: 1.2f);
                    Main.dust[dustIndex].velocity *= 2f;
                }
                for (int i = 0; i < 4; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/BloatedSwarmerGore" + (i + 1)).Type, 1);
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Burst);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Burst = reader.ReadBoolean();
        }
        private bool Burst;
        public override bool PreAI()
        {
            Player player = Main.player[NPC.target];
            NPC.LookByVelocity();
            if (Main.rand.NextBool(400) && !Burst && NPC.DistanceSQ(player.Center) < 400 * 400 && NPC.CountNPCS(ModContent.NPCType<BloatedGrub>()) < 30)
            {
                Burst = true;
                AITimer = 0;
                NPC.netUpdate = true;
            }
            if (Burst)
            {
                NPC.velocity *= 0.96f;
                if (AITimer++ >= 60)
                {
                    SoundEngine.PlaySound(SoundID.Zombie44, NPC.position);
                    for (int i = 0; i < 30; i++)
                        Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenBlood);
                    for (int i = 0; i < Main.rand.Next(3, 9); i++)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int n = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.RandAreaInEntity().X, (int)NPC.RandAreaInEntity().Y, ModContent.NPCType<BloatedGrub>());
                            Main.npc[n].velocity = RedeHelper.Spread(10);
                            if (Main.netMode == NetmodeID.Server && n < Main.maxNPCs)
                            {
                                NetMessage.SendData(MessageID.SyncNPC, number: n);
                            }
                        }
                    }
                    AITimer = 0;
                    Burst = false;
                    NPC.netUpdate = true;
                }
                return false;
            }
            return true;
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 4)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 7 * frameHeight)
                    NPC.frame.Y = 0;
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.BeatAnyMechBoss(), ModContent.ItemType<Xenomite>(), 4, 2, 4));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ToxicBile>(), 4, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GasMask>(), 20));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DoubleRifle>(), 100));
            npcLoot.Add(ItemDropRule.OneFromOptions(50, ModContent.ItemType<IntruderMask>(), ModContent.ItemType<IntruderArmour>(), ModContent.ItemType<IntruderPants>()));
            npcLoot.Add(ItemDropRule.Food(ModContent.ItemType<StarliteDonut>(), 150));
            var dropRules = Main.ItemDropsDB.GetRulesForNPCID(NPCID.FlyingAntlion, false);
            foreach (var dropRule in dropRules)
            {
                npcLoot.Add(dropRule);
            }
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (Main.rand.NextBool(2) || Main.expertMode)
                target.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), Main.rand.Next(200, 1200));
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.BloatedSwarmer"))
            });
        }
    }
}
