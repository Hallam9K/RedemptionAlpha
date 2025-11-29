using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using Redemption.Items.Armor.Vanity;
using Redemption.NPCs.Bosses.Neb.Phase2;
using Redemption.NPCs.Friendly;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Neb.Clone
{
    [AutoloadBossHead]
    public class Nebuleus2_Clone : Nebuleus2
    {
        public override string Texture => "Redemption/NPCs/Bosses/Neb/Phase2/Nebuleus2";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Nebuleus Mirage");
            Main.npcFrameCount[NPC.type] = 9;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn2] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.CursedInferno] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.ShadowFlame] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Venom] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.DryadsWardDebuff] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Electrified] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Bleeding] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffType<PureChillDebuff>()] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffType<DragonblazeDebuff>()] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffType<MoonflareDebuff>()] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCCelestial[Type] = true;
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            NPC nPC = new();
            nPC.SetDefaults(NPCType<Nebuleus>());
            Main.BestiaryTracker.Kills.RegisterKill(nPC);

            potionType = ItemID.SuperHealingPotion;
            if (!Main.expertMode && Main.rand.NextBool(7))
            {
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ItemType<NebuleusMask>());
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ItemType<NebuleusVanity>());
            }

            NPC.SetEventFlagCleared(ref RedeBossDowned.downedNebuleus, -1);
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            if (RedeBossDowned.nebDeath < 7)
            {
                RedeBossDowned.nebDeath = 7;
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.WorldData);
            }
        }
        public override bool ModifyDeathMessage(ref NetworkText customText, ref Color color) => true;
        public override bool CheckDead() => true;
    }
}