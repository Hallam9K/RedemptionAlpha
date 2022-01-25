using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Redemption.Items.Materials.PreHM;
using Microsoft.Xna.Framework;
using Redemption.Globals.Player;
using System;

namespace Redemption.Items.Armor.PreHM.LivingWood
{
    [AutoloadEquip(EquipType.Head)]
    public class LivingWoodHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living Wood Helmet");
            Tooltip.SetDefault("+1 increased druidic damage");
            ArmorIDs.Head.Sets.DrawHead[Mod.GetEquipSlot(Name, EquipType.Head)] = false;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 26;
            Item.sellPrice(copper: 30);
            Item.rare = ItemRarityID.White;
            Item.defense = 1;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<LivingWoodBody>() && legs.type == ModContent.ItemType<LivingWoodLeggings>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<BuffPlayer>().DruidDamageFlat += 1;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Nature Guardians additionally drop small seeds that grow into living wood saplings";

            if (Main.rand.NextBool(25) && Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y) > 1f && !player.rocketFrame)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                Gore.NewGore(new Vector2(player.Center.X + Main.rand.Next(-12, 4), player.Center.Y + Main.rand.Next(6)), player.velocity, GoreID.TreeLeaf_Normal);
            }
        }
    }
}