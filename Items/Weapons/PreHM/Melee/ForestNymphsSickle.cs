using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class ForestNymphsSickle : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Forest Nymph's Sickle");
            /* Tooltip.SetDefault("Deals 50% more damage to dark enemies\n" +
                "Right-click to swap between Melee and Magic abilities"); */
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 62;
            Item.height = 76;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 5);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 27;
            Item.knockBack = 6;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Default;
            Item.noMelee = true;
            Item.channel = true;

            // Projectile Properties
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<ForestNymphsSickle_Proj>();
        }
        private bool MagicMode;
        public override bool AltFunctionUse(Player player) => true;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                player.itemAnimationMax = 5;
                player.itemTime = 5;
                player.itemAnimation = 5;

                for (int i = 0; i < 20; i++)
                {
                    int dustIndex = Dust.NewDust(new Vector2(player.position.X, player.Bottom.Y - 2), player.width, 2, DustID.DryadsWard);
                    Main.dust[dustIndex].velocity.Y = -Main.rand.Next(3, 7);
                    Main.dust[dustIndex].velocity.X = 0;
                    Main.dust[dustIndex].noGravity = true;
                }
                SoundEngine.PlaySound(SoundID.DD2_DarkMageCastHeal);
                MagicMode = !MagicMode;
                return false;
            }
            if (MagicMode)
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 2);
                return false;
            }
            return true;
        }
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage = damage.CombineWith(player.GetTotalDamage(MagicMode ? DamageClass.Magic : DamageClass.Melee));
            if (MagicMode)
                damage *= .7f;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit)
        {
            if (!MagicMode)
                crit += player.GetTotalCritChance(DamageClass.Melee);
            else
                crit += player.GetTotalCritChance(DamageClass.Magic);
        }
        public override void ModifyWeaponKnockback(Player player, ref StatModifier knockback)
        {
            knockback = knockback.CombineWith(player.GetTotalKnockback(MagicMode ? DamageClass.Magic : DamageClass.Melee));
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.Mod == "Terraria");
            if (tt != null)
            {
                string[] splitText = tt.Text.Split(' ');
                string damageValue = splitText.First();
                string damageWord = splitText.Last();
                string damageType = " melee ";
                if (MagicMode)
                    damageType = " magic ";
                tt.Text = damageValue + damageType + damageWord;
            }
            int tooltipLocation = tooltips.FindIndex(TooltipLine => TooltipLine.Name.Equals("Tooltip0"));
            if (tooltipLocation != -1)
            {
                if (!MagicMode)
                    tooltips.Insert(tooltipLocation, new TooltipLine(Mod, "Tooltip", Language.GetTextValue("Mods.Redemption.Items.ForestNymphsSickle.MeleeText")));
                else
                    tooltips.Insert(tooltipLocation, new TooltipLine(Mod, "Tooltip", Language.GetTextValue("Mods.Redemption.Items.ForestNymphsSickle.MagicText")));
            }
            if (!MagicMode)
            {
                TooltipLine slashLine = new(Mod, "SharpBonus", Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.SlashBonus")) { OverrideColor = Colors.RarityOrange };
                tooltips.Add(slashLine);
            }
        }
    }
}
