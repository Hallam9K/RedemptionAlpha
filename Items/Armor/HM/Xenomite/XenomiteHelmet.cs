using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Redemption.Items.Materials.HM;
using Redemption.BaseExtension;
using Redemption.Globals.Player;

namespace Redemption.Items.Armor.HM.Xenomite
{
    [AutoloadEquip(EquipType.Head)]
    public class XenomiteHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("7% increased damage\n" +
            "10% increased critical strike chance");

            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 26;
            Item.sellPrice(silver: 50);
            Item.rare = ItemRarityID.Lime;
            Item.defense = 10;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += .07f;
            player.GetCritChance(DamageClass.Generic) += 10;
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<XenomitePlate>() && legs.type == ModContent.ItemType<XenomiteLeggings>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TitaniumBar, 5)
                .AddIngredient(ModContent.ItemType<XenomiteItem>(), 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.AdamantiteBar, 5)
                .AddIngredient(ModContent.ItemType<XenomiteItem>(), 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Increased Energy regeneration if an Energy Pack is in your inventory\n" +
                "Select a keybind for [Special Ability Key] in Controls";
            foreach (string key in Redemption.RedeSpecialAbility.GetAssignedKeys())
            {
                player.setBonus = "Increased Energy regeneration if an Energy Pack is in your inventory" +
                    "\nPress " + key + " to unleash toxic gas in a radius around the player, inflicting Burning Acid";
            }
            player.GetModPlayer<EnergyPlayer>().energyRegen += 10;
            player.RedemptionPlayerBuff().xenomiteBonus = true;
        }
    }
}