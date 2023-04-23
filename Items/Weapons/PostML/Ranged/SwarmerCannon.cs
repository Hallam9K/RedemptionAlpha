using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Projectiles.Ranged;
using Redemption.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Ranged
{
    public class SwarmerCannon : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.PoisonS);
        public override void SetStaticDefaults()
		{
            /* Tooltip.SetDefault("Holding left-click will grow a hive cyst inside the cannon\n" +
                "Release when the cyst is fully grown to launch it at enemies, dealing " + ElementID.PoisonS + " damage\n" +
                "Replaces normal bullets with bile bullets"); */
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 160;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 64;
            Item.height = 36;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.reuseDelay = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 2;
            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.rare = ModContent.RarityType<TurquoiseRarity>();
            Item.UseSound = SoundID.Item38;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SwarmGrowth_Proj>();
            Item.shootSpeed = 90;
            Item.useAmmo = AmmoID.Bullet;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
        }
        public override bool CanConsumeAmmo(Item ammo, Player player) => false;
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<SwarmerCannon_Proj>();
        }
    }
}
