using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Redemption.Projectiles.Pets;
using Redemption.Buffs.Pets;

namespace Redemption.Items.Accessories.PreHM
{
    public class DevilsAdvocate : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Devil's Advocate");
			// Tooltip.SetDefault("Summons a tiny angel and devil to follow you");

			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToVanitypet(ModContent.ProjectileType<AngelErhan_Proj>(), ModContent.BuffType<ErhanPetBuff>());
			Item.width = 36;
			Item.height = 28;
			Item.rare = ItemRarityID.Master;
			Item.master = true;
			Item.value = Item.sellPrice(0, 5);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			player.AddBuff(Item.buffType, 2);
			return false;
		}
	}
}