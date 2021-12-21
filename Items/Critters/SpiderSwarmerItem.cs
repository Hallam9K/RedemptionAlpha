using Redemption.NPCs.Critters;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Critters
{
    public class SpiderSwarmerItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spider Swarmer");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 12;
            Item.rare = ItemRarityID.Blue;
            Item.maxStack = 999;
            Item.bait = 10;
            Item.value = Item.buyPrice(silver: 1);
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
        }

        public override bool? UseItem(Player player)
        {
            int index = NPC.NewNPC((int) (player.position.X + Main.rand.Next(-20, 20)), (int) (player.position.Y - 0f),
                ModContent.NPCType<SpiderSwarmer>());

            if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
                NetMessage.SendData(MessageID.SyncNPC, number: index);

            return true;
        }
    }
}