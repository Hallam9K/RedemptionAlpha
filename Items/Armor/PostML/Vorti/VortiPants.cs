using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Redemption.Rarities;
using Terraria.ID;

namespace Redemption.Items.Armor.PostML.Vorti
{
    [AutoloadEquip(EquipType.Legs)]
    public class VortiPants : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Legs.Sets.HidesBottomSkin[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs)] = true;
            ArmorIDs.Legs.Sets.IncompatibleWithFrogLeg[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs)] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 18;
            Item.sellPrice(gold: 5);
            Item.rare = ModContent.RarityType<TurquoiseRarity>();
            Item.defense = 24;
        }
    }
}