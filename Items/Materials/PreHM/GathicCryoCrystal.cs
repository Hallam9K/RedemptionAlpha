using Microsoft.Xna.Framework;
using Redemption.Tiles.Natural;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PreHM
{
    public class GathicCryoCrystal : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gathic Cryo-Crystal");
            /* Tooltip.SetDefault("Makes the player chilled when held\n" +
                "'A freezing cold crystal'"); */
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 70;
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 5, 0);
            Item.rare = ItemRarityID.Orange;
            Item.consumable = true;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<CryoCrystalTile>();
        }

        public override void PostUpdate()
        {
            if (!Main.rand.NextBool(10))
                return;

            int sparkle = Dust.NewDust(new Vector2(Item.position.X, Item.position.Y), Item.width, Item.height,
                DustID.SilverCoin, 0, 0, 20);
            Main.dust[sparkle].velocity *= 0;
            Main.dust[sparkle].noGravity = true;
        }

        public override void HoldItem(Player player)
        {
            player.AddBuff(BuffID.Chilled, 60);
        }
    }
}
