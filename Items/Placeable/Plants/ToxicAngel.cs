using Redemption.Globals;
using Redemption.Tiles.Plants;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Plants
{
    public class ToxicAngel : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Toxic Angel");
            SacrificeTotal = 5;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ToxicAngelTile>());
            Item.width = 26;
            Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Lime;
        }
        public override void HoldItem(Player player)
        {
            if (!player.IsRadiationProtected())
            {
                player.AddBuff(BuffID.Suffocation, 50);
                player.AddBuff(BuffID.Obstructed, 10);
            }
        }
        public override void PostUpdate()
        {
            if (Item.wet && !Item.lavaWet && !Item.honeyWet)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int g = 0; g < 3; g++)
                    {
                        int goreIndex = Gore.NewGore(Item.GetSource_FromThis(), Item.Center, default, Main.rand.Next(61, 64));
                        Main.gore[goreIndex].scale = 1.5f;
                        Main.gore[goreIndex].velocity *= 2f;
                    }
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Item.NewItem(Item.GetSource_FromThis(), Item.getRect(), ModContent.ItemType<ToxicAngel2>(), Item.stack, prefixGiven: Item.prefix);
                    Item.active = false;
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, Item.whoAmI);
                }
            }
        }
    }
}
