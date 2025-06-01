using Redemption.BaseExtension;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Items.Placeable.Banners;
using Redemption.Items.Usable.Potions;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.NPCs.Critters
{
    public class ViciousChicken : BaseChicken
    {
        protected override int FeatherType => ModContent.DustType<ChickenFeatherDust6>();
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
            NPC.defense = 5;
            NPC.damage = 21;
            NPC.lifeMax = 80;
            NPC.value = 500;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<ViciousChickenBanner>();
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => AIState is ActionState.Alert;
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.BloodMoon,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.ViciousChicken"))
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.RottenEgg, 1, 1, 2));
            npcLoot.Add(ItemDropRule.ByCondition(new OnFireCondition(), ModContent.ItemType<FriedChicken>()));
        }
        public override void FindFrame(int frameHeight)
        {
            if (AIState is ActionState.Peck)
            {
                NPC.rotation = 0;

                if (NPC.frame.Y < 9 * frameHeight)
                    NPC.frame.Y = 9 * frameHeight;

                NPC.frameCounter++;
                if (NPC.frameCounter >= 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 12 * frameHeight)
                        NPC.frame.Y = 9 * frameHeight;
                }
                return;
            }

            if (NPC.collideY || NPC.velocity.Y == 0)
            {
                NPC.rotation = 0;
                if (NPC.velocity.X == 0)
                    NPC.frame.Y = 0;
                else
                {
                    if (NPC.frame.Y < 1 * frameHeight)
                        NPC.frame.Y = 1 * frameHeight;

                    NPC.frameCounter += NPC.velocity.X * 0.75f;
                    if (NPC.frameCounter is >= 5 or <= -5)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 8 * frameHeight)
                            NPC.frame.Y = 1 * frameHeight;
                    }
                }
            }
            else
            {
                NPC.rotation = NPC.velocity.X * 0.05f;
                NPC.frame.Y = 2 * frameHeight;
            }
        }
    }
}