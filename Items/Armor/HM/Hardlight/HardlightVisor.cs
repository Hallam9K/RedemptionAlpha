using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Redemption.Globals.Player;
using Redemption.Items.Materials.HM;

namespace Redemption.Items.Armor.HM.Hardlight
{
    [AutoloadEquip(EquipType.Head)]
    public class HardlightVisor : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("13% increased ranged damage\n" +
            "5% increased ranged critical strike chance");

            ArmorIDs.Head.Sets.DrawFullHair[Mod.GetEquipSlot(Name, EquipType.Head)] = true;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 16;
            Item.sellPrice(silver: 75);
            Item.rare = ItemRarityID.Cyan;
            Item.defense = 16;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Ranged) += .13f;
            player.GetCritChance(DamageClass.Ranged) += 5;
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
            player.setBonus = "Select a keybind for [Special Ability Key] in Controls";
            foreach (string key in Redemption.RedeSpecialAbility.GetAssignedKeys())
            {
                player.setBonus = "Press " + key + " to get support from the Ship of the Slayer\n" +
                    "Fires a missile barrage from the SoS, targetting the enemy nearest to the cursor position";
            }
            player.GetModPlayer<BuffPlayer>().hardlightBonus = 6;
        }
    }
}