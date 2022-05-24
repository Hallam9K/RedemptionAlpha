using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.DataStructures;
using System.Collections.Generic;
using Redemption.Projectiles.Magic;
using Terraria.GameContent.Creative;
using Redemption.Globals;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Items.Materials.HM;

namespace Redemption.Items.Weapons.HM.Magic
{
    public class GlobalDischarge : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Casts an unstable lightning sphere, contained as long as the staff's stream is active\n" +
                "Tap left-click to cast and immediately break the stream, causing an unstable discharge" +
                "\nHold down left-click to cast and keep the sphere stable, constantly emitting lightning at an enemy at cursor point\n" +
                "Keeping the sphere active for too long will cause the user to take damage as well");
            Item.staff[Item.type] = true;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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
            Item.rare = ItemRarityID.Yellow;
            Item.noMelee = true;
            Item.autoReuse = false;
            Item.noUseGraphic = true;
            Item.value = Item.buyPrice(0, 65, 50, 0);
            Item.UseSound = new("Redemption/Sounds/Custom/ElectricSlash2");
            Item.shootSpeed = 16f;
            Item.shoot = ModContent.ProjectileType<GlobalDischarge_Sphere>();
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Item.ModItem.Texture + "_Glow").Value;
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
