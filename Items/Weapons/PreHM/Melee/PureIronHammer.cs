using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Materials.PreHM;
using Terraria.GameContent.Creative;

namespace Redemption.Items.Weapons.PreHM.Melee
{
	public class PureIronHammer : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Pure-Iron Hammer");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 38;
			Item.DamageType = DamageClass.Melee;
			Item.width = 50;
			Item.height = 50;
			Item.useTime = 22;
			Item.useAnimation = 38;
			Item.hammer = 70;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
			Item.value = 1050;
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<PureIronBar>(), 16)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}