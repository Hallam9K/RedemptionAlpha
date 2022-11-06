using Redemption.NPCs.Critters;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Critters
{
    public class CoastScarabItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Coast Scarab");

            SacrificeTotal = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 14;
            Item.rare = ItemRarityID.Blue;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(silver: 4);
            Item.bait = 10;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
        }

        public override bool? UseItem(Player player)
        {
            int index = NPC.NewNPC(new EntitySource_SpawnNPC(), (int) (player.position.X + Main.rand.Next(-20, 20)), (int) (player.position.Y - 0f),
                ModContent.NPCType<CoastScarab>());

            if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
                NetMessage.SendData(MessageID.SyncNPC, number: index);

            return true;
        }
    }
}