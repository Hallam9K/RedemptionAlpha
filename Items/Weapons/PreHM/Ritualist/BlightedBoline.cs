using Microsoft.Xna.Framework;
using Redemption.DamageClasses;
using Redemption.Globals.Player;
using Redemption.Projectiles.Ritualist;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Ritualist
{
    public class BlightedBoline : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blighted Boline");
            // Tooltip.SetDefault("After the player's Spirit Level increases, the following swing fires a cursed thorn tenril");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 30;
            Item.height = 30;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 25);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 16;
            Item.useTime = 16;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 17;
            Item.knockBack = 5;
            Item.noUseGraphic = true;
            Item.DamageType = ModContent.GetInstance<RitualistClass>();
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<BlightedBoline_Slash>();
        }
        private int oldSpiritLevel;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (oldSpiritLevel < player.GetModPlayer<RitualistPlayer>().SpiritLevel)
                Projectile.NewProjectile(source, position, velocity * 2f, ModContent.ProjectileType<CursedThornVileF>(), damage, knockback, player.whoAmI);

            oldSpiritLevel = player.GetModPlayer<RitualistPlayer>().SpiritLevel;
            return true;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 Offset = Vector2.Normalize(velocity) * 50f;
            position += Offset;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine slashLine = new(Mod, "SharpBonus", Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.SlashBonus")) { OverrideColor = Colors.RarityOrange };
            tooltips.Add(slashLine);
        }
    }
}
