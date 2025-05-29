using Redemption.Globals;
using Redemption.Items.Materials.HM;
using Redemption.Projectiles.Ranged;
using Redemption.UI;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Ranged
{
    public class LegoPistol : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 100;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 46;
            Item.height = 40;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Gray;
            Item.UseSound = SoundID.Item40;
            Item.autoReuse = false;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 16f;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.useAmmo = AmmoID.Bullet;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.LegoBreak, player.position);
            if (Main.netMode != NetmodeID.Server)
            {
                for (int i = 0; i < 6; i++)
                    Gore.NewGore(source, position, RedeHelper.Spread(5), Find<ModGore>("Redemption/LegoPistolPiece" + (i + 1)).Type, 1);
            }
            return true;
        }
    }
}
