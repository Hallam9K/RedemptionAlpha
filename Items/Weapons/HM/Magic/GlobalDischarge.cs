using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;

namespace Redemption.Items.Weapons.HM.Magic
{
    public class GlobalDischarge : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Casts an unstable lightning sphere, contained as long as the staff's stream is active\n" +
                "Tap left-click to cast and immediately break the stream, causing an unstable discharge" +
                "\nHold down left-click to cast and keep the sphere stable, using momentum to swing it" +
                "\nDeals more damage the faster it moves"); */
            Item.staff[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 68;
            Item.height = 48;
            Item.width = 50;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.reuseDelay = 80;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 45;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3;
            Item.channel = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.noMelee = true;
            Item.autoReuse = false;
            Item.noUseGraphic = true;
            Item.value = Item.sellPrice(0, 6, 0, 0);
            Item.UseSound = CustomSounds.ElectricSlash2;
            Item.shootSpeed = 16f;
            Item.shoot = ModContent.ProjectileType<GlobalDischarge_Sphere>();
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] == 0;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<GlobalDischarge_Proj>();
        }
    }
}