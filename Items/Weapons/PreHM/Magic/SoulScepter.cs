using Microsoft.Xna.Framework;
using Redemption.Projectiles.Magic;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Magic
{
    public class SoulScepter : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Casts controllable soul-charges that orbit the cursor" +
                "\nMore soul-charges are cast the longer you hold");
            Item.staff[Item.type] = true;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 14;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 5;
            Item.width = 38;
            Item.height = 44;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1;
            Item.value = 2000;
            Item.channel = true;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item9.WithVolume(0);
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SoulScepterChargeS>();
            Item.shootSpeed = 10f;
        }
        public override bool CanShoot(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
    }
}