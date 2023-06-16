using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Redemption.BaseExtension;
using Terraria.DataStructures;
using Redemption.Globals;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PostML;

namespace Redemption.Items.Weapons.PostML.Ranged
{
    public class BlastBattery : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("'Prepare for obliteration'"
                + "\nLeft-Click to mark a single enemy and fire a stream of missiles at their position" +
                "\nRight-Click to mark your cursor position with a barrage of missiles" +
                "\nUses rockets as ammo"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 160;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 30;
            Item.height = 38;
            Item.useTime = 5;
            Item.useAnimation = 30;
            Item.reuseDelay = 60;
            Item.knockBack = 7;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = CustomSounds.AlarmItem;
            Item.autoReuse = false;
            Item.useTurn = true;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.shoot = ModContent.ProjectileType<BlastBattery_Missile>();
            Item.useAmmo = AmmoID.Rocket;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
        }
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            NPC target = null;
            if (player.altFunctionUse != 2 && !RedeHelper.ClosestNPC(ref target, 300, Main.MouseWorld, true, player.MinionAttackTargetNPC))
                return false;
            return true;
        }
        public override void HoldItem(Player player)
        {
            player.RedemptionPlayerBuff().blastBattery = true;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
                Item.useTime = 5;
            else
                Item.useTime = 30;
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<BlastBattery_Crosshair>();
            position = Main.MouseWorld;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
                Projectile.NewProjectile(source, position + RedeHelper.Spread(80), Vector2.Zero, type, damage, knockback, Main.myPlayer, 1);
            else
                return true;

            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<RoboBrain>())
                .AddIngredient(ModContent.ItemType<OmegaPowerCell>(), 2)
                .AddIngredient(ModContent.ItemType<CorruptedXenomite>(), 12)
                .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 4)
                .AddIngredient(ModContent.ItemType<Plating>(), 2)
                .AddIngredient(ModContent.ItemType<Capacitor>())
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
