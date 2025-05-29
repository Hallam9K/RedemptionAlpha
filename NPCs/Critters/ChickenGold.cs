using Microsoft.Xna.Framework;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Items.Critters;
using Redemption.Items.Usable.Potions;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs.Critters
{
    public class ChickenGold : BaseChicken
    {
        protected override int FeatherType => ModContent.DustType<ChickenFeatherDust5>();
        protected override bool Evil => false;
        protected override bool Normal => false;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gold Chicken");
            Main.npcFrameCount[Type] = 21;
            NPCID.Sets.ShimmerTransformToNPC[NPC.type] = NPCID.Shimmerfly;
            NPCID.Sets.CountsAsCritter[Type] = true;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;
            NPCID.Sets.NormalGoldCritterBestiaryPriority.Add(Type);
            NPCID.Sets.GoldCrittersCollection.Add(Type);

            NPCID.Sets.NPCBestiaryDrawModifiers value = new() { Velocity = 1f };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.catchItem = (short)ModContent.ItemType<ChickenGoldItem>();
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.ByCondition(new OnFireCondition(), ModContent.ItemType<FriedChicken>()));
        }
        public override void PostAI()
        {
            if (!Main.rand.NextBool(20))
                return;

            int sparkle = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.GoldCoin, 0, 0, 20);
            Main.dust[sparkle].velocity *= 0;
            Main.dust[sparkle].noGravity = true;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new GoldCritterUICollectionInfoProvider(new int[] { ModContent.NPCType<Chicken>() },
                ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[NPC.type]);

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.ChickenGold"))
            });
        }
    }
}