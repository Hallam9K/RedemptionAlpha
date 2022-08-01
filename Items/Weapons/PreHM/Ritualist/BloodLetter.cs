using Microsoft.Xna.Framework;
using Redemption.DamageClasses;
using Redemption.Globals.Player;
using Redemption.Items.Materials.PreHM;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Ritualist
{
    public class BloodLetter : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Holding left-click will drain the player's Spirit Gauge in exchange for increased life regeneration\n" +
                "Right-click for a normal slash");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 26;
            Item.height = 26;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 75);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.channel = true;

            // Weapon Properties
            Item.damage = 22;
            Item.knockBack = 3;
            Item.noUseGraphic = true;
            Item.DamageType = ModContent.GetInstance<RitualistClass>();
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<BloodLetter_Slash>();
        }
        public override bool AltFunctionUse(Player player) => true;
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse != 2 && (player.GetModPlayer<RitualistPlayer>().SpiritLevel > 0 || player.GetModPlayer<RitualistPlayer>().SpiritGauge > 0))
            {
                type = ModContent.ProjectileType<BloodLetter_Proj>();
            }
            else
            {
                type = Item.shoot;
                velocity += velocity.RotatedByRandom(0.8f);
                Vector2 Offset = Vector2.Normalize(velocity) * 50f;
                position += Offset;
            }
        }
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse != 2)
                return player.GetModPlayer<RitualistPlayer>().SpiritLevel > 0 || player.GetModPlayer<RitualistPlayer>().SpiritGauge > 0;

            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CrimtaneBar, 10)
                .AddIngredient(ItemID.TissueSample, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
