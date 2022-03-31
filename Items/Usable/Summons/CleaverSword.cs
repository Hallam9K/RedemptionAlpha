using Redemption.NPCs.Bosses.Cleaver;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Summons
{
    public class CleaverSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Omega Blade");
            Tooltip.SetDefault("Summons the 1st Omega Prototype" +
                "\n'The corrupted blade draws near the power, thus beginning the final hour'"
                + "\nOnly usable at night"
                + "\nNot consumable");
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 13;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 38;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 15, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }
        public override bool CanUseItem(Player player)
        {
            return !Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<OmegaCleaver>()) && !NPC.AnyNPCs(ModContent.NPCType<Wielder>());
        }
        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.position, 0);

            int type = ModContent.NPCType<Wielder>();

            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.NewNPC(new EntitySource_BossSpawn(player), (int)player.position.X + 200, (int)player.position.Y + 500, type);
            return true;
        }
    }
}