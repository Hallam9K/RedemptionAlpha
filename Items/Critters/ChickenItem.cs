using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Redemption.NPCs.Critters;
using Terraria.DataStructures;

namespace Redemption.Items.Critters
{
    public class ChickenItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chicken");

            SacrificeTotal = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 26;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(silver: 5);
            Item.rare = ItemRarityID.Blue;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
        }
        public override bool? UseItem(Player player)
        {
            int index = NPC.NewNPC(new EntitySource_SpawnNPC(), (int)(player.position.X + Main.rand.Next(-20, 20)), (int)(player.position.Y - 0f),
                ModContent.NPCType<Chicken>(), ai1: -1);

            if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
                NetMessage.SendData(MessageID.SyncNPC, number: index);

            return true;
        }
    }
}
