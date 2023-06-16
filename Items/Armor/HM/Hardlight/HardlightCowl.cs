using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Materials.HM;
using Redemption.BaseExtension;
using Terraria.Localization;

namespace Redemption.Items.Armor.HM.Hardlight
{
    [AutoloadEquip(EquipType.Head)]
    public class HardlightCowl : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("13% increased magic damage\n" +
            "5% increased magic critical strike chance\n" +
            "+50 max mana"); */

            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 18;
            Item.sellPrice(silver: 75);
            Item.rare = ItemRarityID.LightPurple;
            Item.defense = 16;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Magic) += .13f;
            player.GetCritChance(DamageClass.Magic) += 5;
            player.statManaMax2 += 50;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<HardlightPlate>() && legs.type == ModContent.ItemType<HardlightBoots>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<CyberPlating>(), 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.Hardlight.Keybind");
            foreach (string key in Redemption.RedeSpecialAbility.GetAssignedKeys())
            {
                player.setBonus = Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.Hardlight.Press") + key + Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.Hardlight.Support") +
                    Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.Hardlight.Cowl");
            }
            player.RedemptionPlayerBuff().hardlightBonus = 2;
            player.RedemptionPlayerBuff().MetalSet = true;
        }
    }
}
