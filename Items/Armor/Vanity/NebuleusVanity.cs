using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Redemption.Rarities;

namespace Redemption.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Body)]
	public class NebuleusVanity : ModItem
	{
        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
        }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Nebuleus Robes");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
		{
			Item.width = 38;
            Item.height = 40;
            Item.vanity = true;
            Item.rare = ModContent.RarityType<CosmicRarity>();
        }
        public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
        {
            robes = true;
            equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
        }
    }
}
