using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Globals.Player;
using Redemption.Items.Weapons.HM.Summon;
using Redemption.Projectiles.Ranged;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Ranged
{
    public class PrototypeAtomRifle : ModItem
    {
        public override void SetStaticDefaults()
        {
            ElementID.ItemFire[Type] = true;
            ElementID.ItemExplosive[Type] = true;

            ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<TeslaGenerator>();
        }

        public override void SetDefaults()
        {
            Item.damage = 94;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 94;
            Item.height = 40;
            Item.useTime = 48;
            Item.useAnimation = 48;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0.5f;
            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = CustomSounds.PlasmaShot;

            Item.autoReuse = true;
            Item.shoot = ProjectileType<PlasmaRound>();
            Item.shootSpeed = 12;
            Item.useAmmo = AmmoID.Bullet;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
        }
        public override bool CanUseItem(Player player)
        {
            return player.GetModPlayer<EnergyPlayer>().statEnergy >= 4;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-16, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.GetModPlayer<EnergyPlayer>().statEnergy -= 4;
            int proj = Projectile.NewProjectile(source, position, velocity, ProjectileType<PlasmaRound>(), damage, knockback, player.whoAmI);
            Main.projectile[proj].hostile = false;
            Main.projectile[proj].friendly = true;
            Main.projectile[proj].DamageType = DamageClass.Ranged;
            Main.projectile[proj].tileCollide = true;
            Main.projectile[proj].netUpdate = true;
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.Items.PrototypeAtomRifle.Lore"))
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
