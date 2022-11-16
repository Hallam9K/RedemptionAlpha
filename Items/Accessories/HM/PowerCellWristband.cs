using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.HM
{
    public class PowerCellWristband : ModItem
	{
		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Power Cell Wristband");
            Tooltip.SetDefault("4% increased critical strike chance for Fire and Holy elemental weapons\n" +
                "Stacks if both elements are present\n" +
                "An aura of fire surrounds you while holding a Fire or Holy elemental weapon\n" +
                "'Fueled with the sun itself'");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
		{
            Item.width = 24;
            Item.height = 12;
            Item.value = Item.sellPrice(0, 6, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Item.ModItem.Texture + "_Glow").Value;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Leather, 5)
                .AddIngredient(ItemID.LunarTabletFragment, 2)
                .AddIngredient(ItemID.LihzahrdPowerCell, 3)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        private int timer;
        public override void UpdateAccessory(Player player, bool hideVisual)
		{
            if (player.whoAmI == Main.myPlayer && player.active && !player.dead && (ItemLists.Fire.Contains(player.HeldItem.type) || ProjectileLists.Fire.Contains(player.HeldItem.shoot) || ItemLists.Holy.Contains(player.HeldItem.type) || ProjectileLists.Holy.Contains(player.HeldItem.shoot)))
            {
                if (timer++ % 30 == 0)
                    RedeDraw.SpawnCirclePulse(player.Center, Color.DarkOrange * 0.8f, 0.8f, player);

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (!npc.active || !npc.CanBeChasedBy() || player.DistanceSQ(npc.Center) > 280 * 280)
                        continue;

                    npc.AddBuff(BuffID.OnFire3, 4);
                }
            }
            player.RedemptionPlayerBuff().powerCell = true;
		}
        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            if (slot < 10)
            {
                int maxAccessoryIndex = 5 + player.extraAccessorySlots;
                for (int i = 3; i < 3 + maxAccessoryIndex; i++)
                {
                    if (slot != i && player.armor[i].type == ModContent.ItemType<GracesGuidance>())
                        return false;
                }
            }
            return true;
        }
    }
}
