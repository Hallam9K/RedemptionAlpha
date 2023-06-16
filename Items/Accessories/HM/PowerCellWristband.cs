using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.HM
{
    public class PowerCellWristband : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.FireS, ElementID.HolyS);
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Power Cell Wristband");
            /* Tooltip.SetDefault("4% increased critical strike chance for " + ElementID.FireS + " and " + ElementID.HolyS + " elemental weapons\n" +
                "Stacks if both elements are present\n" +
                "An aura of fire surrounds you while holding a " + ElementID.FireS + " or " + ElementID.HolyS + " elemental weapon\n" +
                "'Fueled with the sun itself'"); */
            Item.ResearchUnlockCount = 1;
            ElementID.ItemFire[Type] = true;
            ElementID.ItemHoly[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.value = Item.sellPrice(0, 6, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
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
            if (player.whoAmI == Main.myPlayer && player.active && !player.dead && (player.HeldItem.HasElementItem(ElementID.Fire) || player.HeldItem.HasElementItem(ElementID.Holy)))
            {
                if (timer++ % 30 == 0)
                    RedeDraw.SpawnCirclePulse(player.Center, Color.DarkOrange * 0.8f, 0.8f, player);

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (!npc.active || !npc.CanBeChasedBy() || NPCLoader.CanBeHitByItem(npc, player, Item) is false || player.DistanceSQ(npc.Center) > 280 * 280)
                        continue;

                    npc.AddBuff(BuffID.OnFire3, 4);
                }
            }
            player.RedemptionPlayerBuff().powerCell = true;
        }
    }
}
