using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Redemption.Projectiles.Ranged;
using System.Collections.Generic;
using Redemption.Globals;
using Terraria.DataStructures;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.Items.Weapons.PostML.Melee;

namespace Redemption.Items.Weapons.PostML.Ranged
{
    public class DarkSteelBow : ModItem
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Daerel's Dark-Steel Bow");
            /* Tooltip.SetDefault("Shoots Dark-Steel arrows that create shadow tendrils upon hitting a target\n" +
                "20% chance not to consume ammo"); */
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<MythrilsBane>();
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 30;
            Item.height = 50;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.buyPrice(platinum: 5);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 130;
            Item.knockBack = 3;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 20f;
            Item.shoot = ModContent.ProjectileType<DarkSteelArrow>();
            Item.useAmmo = AmmoID.Arrow;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
			return Main.rand.NextFloat() >= .2f;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = Item.shoot;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position + RedeHelper.PolarVector(8, (player.Center - Main.MouseWorld).ToRotation() + MathHelper.PiOver2), velocity, type, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position + RedeHelper.PolarVector(8, (player.Center - Main.MouseWorld).ToRotation() - MathHelper.PiOver2), velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'This bow was forged by the legendary bowsmith Raiktu Shadeheart, and became the reward for a special competition\n" +
                    "held in Arrgath - Erellon's Capital. The winner was Daerel Foremaul, who received this weapon after a close victory.\n" +
                    "The bow is made from rosewood of Erellon's jungles and Dark-Steel, a metal scavenged from the remains of a terrible demon.'")
                {
                    OverrideColor = Color.LightGray
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view lore")
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }
}
