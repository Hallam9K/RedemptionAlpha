using Redemption.Items.Materials.HM;
using Redemption.NPCs.Bosses.Cleaver;
using Redemption.Tiles.Furniture.Lab;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Summons
{
    public class CleaverSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Summon For Cleaver");
            Tooltip.SetDefault("Summons one of Vlitch's Overlords"
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
            Item.value = Item.sellPrice(0, 50, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }
        public override bool CanUseItem(Player player)
        {
            return !Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<VlitchCleaver>()) && !NPC.AnyNPCs(ModContent.NPCType<Wielder>());
        }
        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Roar, player.position, 0);

                int type = ModContent.NPCType<Wielder>();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.NewNPC((int)player.position.X + 200, (int)player.position.Y + 500, type);
                else
                    NetMessage.SendData(MessageID.SpawnBoss, number: player.whoAmI, number2: type);
            }
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<CorruptedStarliteBar>(), 5)
                .AddIngredient(ModContent.ItemType<GirusChip>(), 1)
                .AddTile(ModContent.TileType<GirusCorruptorTile>())
                .Register();
        }
    }
}