using Redemption.NPCs.Critters;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Critters
{
    public class GrandLarvaBait : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grand Larva");

            SacrificeTotal = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 30;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(0, 0, 2);
            Item.rare = ItemRarityID.Blue;
            Item.bait = 55;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
        }

        public override bool? UseItem(Player player)
        {
            int index = NPC.NewNPC(new EntitySource_SpawnNPC(), (int) (player.position.X + Main.rand.Next(-20, 20)), (int) (player.position.Y - 0f),
                ModContent.NPCType<GrandLarva>());

            if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
                NetMessage.SendData(MessageID.SyncNPC, number: index);

            return true;
        }
    }
}