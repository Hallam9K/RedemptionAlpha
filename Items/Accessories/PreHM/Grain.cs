using Microsoft.Xna.Framework;
using Redemption.Buffs.Pets;
using Redemption.Projectiles.Pets;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PreHM
{
    public class Grain : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Pile o' Grain");
			// Tooltip.SetDefault("Summons a pet chicken");

			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToVanitypet(ModContent.ProjectileType<PetChicken>(), ModContent.BuffType<PetChickenBuff>());
			Item.width = 30;
			Item.height = 14;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(0, 0, 1);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			player.AddBuff(Item.buffType, 2);
			return false;
		}
	}
}