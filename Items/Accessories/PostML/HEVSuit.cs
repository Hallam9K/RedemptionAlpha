using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Globals.Player;
using Redemption.Items.Accessories.HM;
using Redemption.Items.Materials.PostML;
using Redemption.Items.Usable.Potions;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PostML
{
    public class HEVSuit : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("HEV Suit");
            Tooltip.SetDefault("Grants immunity to the Abandoned Lab and Wasteland water"
                + "\nGreatly extends underwater breathing"
                + "\nGrants immunity to Radioactive Fallout and all infection debuffs"
                + "\nGrants protection against up to mid-level radiation");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 30;
            Item.value = Item.buyPrice(1, 0, 0, 0);
            Item.rare = ItemRarityID.Purple;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<HeavyRadiationDebuff>()] = true;
            player.buffImmune[ModContent.BuffType<RadioactiveFalloutDebuff>()] = true;
            player.buffImmune[ModContent.BuffType<GreenRashesDebuff>()] = true;
            player.buffImmune[ModContent.BuffType<GlowingPustulesDebuff>()] = true;
            player.buffImmune[ModContent.BuffType<FleshCrystalsDebuff>()] = true;
            player.buffImmune[ModContent.BuffType<ShockDebuff>()] = true;
            player.GetModPlayer<BuffPlayer>().HEVSuit = true;
            player.GetModPlayer<BuffPlayer>().WastelandWaterImmune = true;
            player.accDivingHelm = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RedeRecipe.HazmatSuitRecipeGroup)
                .AddIngredient(ModContent.ItemType<GasMask>())
                .AddIngredient(ModContent.ItemType<CrystalSerum>(), 4)
                .AddIngredient(ModContent.ItemType<RawXenium>(), 4)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}