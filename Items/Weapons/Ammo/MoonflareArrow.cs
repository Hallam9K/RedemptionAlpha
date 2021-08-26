using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Materials.PreHM;
using Redemption.Projectiles.Ranged;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.Ammo
{
    public class MoonflareArrow : ModItem
	{
		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Moonflare Arrow");
            Tooltip.SetDefault("Burns targets while the moon is out" +
				"\nFlame intensity is based on moon phase");
		}

		public override void SetDefaults()
		{
			Item.damage = 5;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 14;
			Item.height = 34;
			Item.maxStack = 999;
			Item.consumable = true;
			Item.knockBack = 2.5f;
			Item.value = 2;
			Item.rare = ItemRarityID.White;
			Item.shoot = ModContent.ProjectileType<MoonflareArrow_Proj>();
			Item.shootSpeed = 7f;
			Item.ammo = AmmoID.Arrow;
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			string text = "There is no moonlight to reflect...";
			if (Main.dayTime || Main.moonPhase == 4)
			{
				TooltipLine line = new(Mod, "text", text)
				{
					overrideColor = Color.LightGray
				};
				tooltips.Insert(2, line);
			}
		}
		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			Texture2D texture = TextureAssets.Item[Item.type].Value;
			Texture2D textureGlow = ModContent.Request<Texture2D>("Redemption/Items/Weapons/Ammo/" + GetType().Name + "_Glow").Value;
			Rectangle frame = texture.Frame();
			Vector2 origin = frame.Size() / 2f;

			spriteBatch.Draw(texture, Item.Center - Main.screenPosition, frame, lightColor, rotation, origin, scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(textureGlow, Item.Center - Main.screenPosition, frame, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);

			return false;
		}
		public override void AddRecipes()
		{
			CreateRecipe(20)
				.AddIngredient(ItemID.WoodenArrow, 20)
				.AddIngredient(ModContent.ItemType<MoonflareFragment>())
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}
