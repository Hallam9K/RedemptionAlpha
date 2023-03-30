using Redemption.NPCs.Critters;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Critters
{
    public class KabucraItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Kabucra");

            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(silver: 6);
            Item.bait = 20;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
        }

        public override bool? UseItem(Player player)
        {
            int index = NPC.NewNPC(new EntitySource_SpawnNPC(), (int) (player.position.X + Main.rand.Next(-20, 20)), (int) (player.position.Y - 0f),
                ModContent.NPCType<Kabucra>());

            if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
                NetMessage.SendData(MessageID.SyncNPC, number: index);

            return true;
        }
    }
}