using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Globals.Players;
using Redemption.Items.Materials.PreHM;
using Redemption.Projectiles.Ranged;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Ranged
{
    public class LunarShot : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.ArcaneS, ElementID.FireS, ElementID.NatureS);
        public override void SetStaticDefaults()
        {
            ElementID.ItemFire[Type] = true;
            ElementID.ItemNature[Type] = true;
            ElementID.ItemArcane[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 9;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 18;
            Item.height = 44;
            Item.useTime = 21;
            Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1;
            Item.value = Item.sellPrice(0, 0, 2, 0);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = false;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 10f;
            Item.useAmmo = AmmoID.Arrow;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;

            Item.Redemption().HideElementTooltip[ElementID.Fire] = true;
            Item.Redemption().HideElementTooltip[ElementID.Nature] = true;
            Item.Redemption().HideElementTooltip[ElementID.Arcane] = true;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.WoodenArrowFriendly)
                type = ProjectileType<LunarShot_Proj>();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string text = "There is no moonlight to reflect...";
            if (Main.dayTime || Main.moonPhase == 4)
            {
                TooltipLine line = new(Mod, "text", text)
                {
                    OverrideColor = Color.LightGray
                };
                tooltips.Insert(2, line);
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<MoonflareFragment>(), 8)
                .AddTile(TileID.Anvils)
                .AddCondition(RedeConditions.InMoonlight)
                .Register();
        }
    }
}
