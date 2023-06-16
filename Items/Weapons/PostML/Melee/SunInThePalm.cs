using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PostML;
using Redemption.Projectiles.Melee;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class SunInThePalm : ModItem
    {
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.HandsOn}", EquipType.HandsOn, Item.ModItem, null, new EquipTexture());
            }
        }
        private void SetupDrawing()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.GetEquipSlot(Mod, Name, EquipType.HandsOn);
            }
        }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sun-In-Palm");
            /* Tooltip.SetDefault("Hold out this mechanical hand to grow a ball of energy\n" +
                "Disintegrates most projectiles after reaching a certain size\n" +
                "Release left-click to shrink it back down, overcharging will cause it to explode and set the player on fire\n" +
                "'The power of the sun, in the palm of my hand'"); */
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            Item.ResearchUnlockCount = 1;
            SetupDrawing();
        }

        public override void SetDefaults()
        {
            Item.damage = 300;
            Item.DamageType = DamageClass.Melee;
            Item.width = 28;
            Item.height = 30;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 5;
            Item.value = Item.sellPrice(0, 11, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = CustomSounds.BallFire;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SunInThePalm_EnergyBall>();
            Item.shootSpeed = 5f;
        }
        public override void HoldItem(Player player)
        {
            var p = player.GetModPlayer<SunInPalmPlayer>();
            p.VanityOn = true;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<SunInThePalm_Proj>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<RoboBrain>())
                .AddIngredient(ModContent.ItemType<OmegaPowerCell>(), 2)
                .AddIngredient(ModContent.ItemType<CorruptedXenomite>(), 9)
                .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 6)
                .AddIngredient(ModContent.ItemType<Plating>(), 2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line2 = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.Items.SunInThePalm.Lore"))
                {
                    OverrideColor = Color.LightGray
                };
                tooltips.Add(line2);
            }
            else
            {
                TooltipLine line2 = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.Items.SunInThePalm.Viewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line2);
            }
        }
    }
    public class SunInPalmPlayer : ModPlayer
    {
        public bool VanityOn;

        public override void ResetEffects()
        {
            VanityOn = false;
        }
        public override void FrameEffects()
        {
            if (VanityOn)
            {
                var item = ModContent.GetInstance<SunInThePalm>();
                Player.handon = (sbyte)EquipLoader.GetEquipSlot(Mod, item.Name, EquipType.HandsOn);
            }
        }
    }
}
