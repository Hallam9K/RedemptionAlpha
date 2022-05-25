using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Redemption.Items.Materials.HM;
using Redemption.DamageClasses;
using Redemption.BaseExtension;

namespace Redemption.Items.Armor.HM.Hardlight
{
    [AutoloadEquip(EquipType.Head)]
    public class HardlightCasque : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("13% increased ritual damage\n" +
            "5% increased ritual critical strike chance\n" +
            "+2 max spirit level");

            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 22;
            Item.sellPrice(silver: 75);
            Item.rare = ItemRarityID.Cyan;
            Item.defense = 16;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<RitualistClass>() += .13f;
            player.GetCritChance<RitualistClass>() += 5;
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
            player.setBonus = "Select a keybind for [Special Ability Key] in Controls"; // TODO: Hardlight ritualist bonus
            foreach (string key in Redemption.RedeSpecialAbility.GetAssignedKeys())
            {
                player.setBonus = "Press " + key + " to get support from the Ship of the Slayer\n" +
                    "Summons a hologram of King Slayer III to help, higher spirit levels give the hologram stronger weapons";
            }
            player.RedemptionPlayerBuff().hardlightBonus = 1;
            player.RedemptionPlayerBuff().MetalSet = true;
        }
    }
}