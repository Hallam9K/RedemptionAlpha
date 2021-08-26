using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Redemption.Items.Materials.PreHM;
using Redemption.Projectiles.Ranged;
using Terraria.GameContent.Creative;
using System.Collections.Generic;

namespace Redemption.Items.Weapons.PreHM.Ranged
{
    public class LunarShot : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lunar Shot");
            Tooltip.SetDefault("Replaces Wooden Arrows with Lunar Bolts" +
                "\nLunar Bolts summon bats while the moon is out" +
                "\nSummons an extra bat while it's a full moon");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
		{
			Item.damage = 9;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 22;
            Item.height = 44;
            Item.useTime = 21;
            Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0;
            Item.value = Item.sellPrice(0, 0, 2, 0);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = false;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 10f;
            Item.useAmmo = AmmoID.Arrow;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.WoodenArrowFriendly)
            {
                type = ModContent.ProjectileType<LunarShot_Proj>();
            }
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
            Texture2D textureGlow = ModContent.Request<Texture2D>("Redemption/Items/Weapons/PreHM/Ranged/" + GetType().Name + "_Glow").Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = frame.Size() / 2f;

            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, frame, lightColor, rotation, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(textureGlow, Item.Center - Main.screenPosition, frame, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);

            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<MoonflareFragment>(), 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
