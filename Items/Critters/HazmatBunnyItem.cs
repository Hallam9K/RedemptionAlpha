using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Redemption.NPCs.Wasteland;
using Terraria.DataStructures;

namespace Redemption.Items.Critters
{
    public class HazmatBunnyItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hazmat Bunny");
            SacrificeTotal = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 26;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(silver: 2);
            Item.rare = ItemRarityID.Blue;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
        }
        public override bool? UseItem(Player player)
        {
            int index = NPC.NewNPC(new EntitySource_SpawnNPC(), (int)(player.position.X + Main.rand.Next(-20, 20)), (int)player.position.Y,
                ModContent.NPCType<HazmatBunny>());

            if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
                NetMessage.SendData(MessageID.SyncNPC, number: index);

            return true;
        }
    }
}
