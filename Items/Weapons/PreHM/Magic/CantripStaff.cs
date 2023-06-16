using Microsoft.Xna.Framework;
using Redemption.Projectiles.Magic;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Magic
{
    public class CantripStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cantrip Staff");
            /* Tooltip.SetDefault("Casts a ball of ember" +
                "\nCasts a larger fireball every 4 consecutive shots"); */
            Item.staff[Item.type] = true;

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 11;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 5;
            Item.width = 36;
            Item.height = 36;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3;
            Item.value = 2500;
            Item.channel = true;
            Item.rare = ItemRarityID.White;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CantripEmber>();
            Item.shootSpeed = 16f;
        }
        private int CastCount;
        public override void HoldItem(Player player)
        {
            if (!player.channel)
                CastCount = 0;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 Offset = Vector2.Normalize(velocity) * 25f;

            if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
            {
                position += Offset;
            }

            CastCount++;
            if (CastCount >= 4)
            {
                type = ModContent.ProjectileType<CantripEmberS>();
                damage *= 2;
                knockback += 5;
                CastCount = 0;
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.Items.CantripStaff.Lore"))
                {
                    OverrideColor = Color.LightGray
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.Viewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }
}
