using Redemption.BaseExtension;
using Redemption.Dusts;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs.Critters
{
    public class CorruptChicken : ViciousChicken
    {
        protected override int FeatherType => ModContent.DustType<ChickenFeatherDust7>();
        protected override bool Evil => true;
        protected override bool Normal => false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 13;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Velocity = 1 };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.defense = 4;
            NPC.damage = 19;
            NPC.lifeMax = 75;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.BloodMoon,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.CorruptChicken"))
            });
        }
    }
}