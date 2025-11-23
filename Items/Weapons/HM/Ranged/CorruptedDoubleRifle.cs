using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Globals.Players;
using Redemption.Items.Materials.HM;
using Redemption.Projectiles.Ranged;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Ranged
{
    public class CorruptedDoubleRifle : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.ThunderS);
        public override void SetStaticDefaults()
        {
            ElementID.ItemThunder[Type] = true;
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 58;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 76;
            Item.height = 32;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item36;
            Item.autoReuse = true;
            Item.shootSpeed = 90;
            Item.shoot = ItemID.PurificationPowder;
            Item.useAmmo = AmmoID.Bullet;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = Request<Texture2D>(Texture + "_Glow").Value;

            Item.Redemption().HideElementTooltip[ElementID.Thunder] = true;
        }
        public int Count;
        public bool Charged;
        public bool Ready;
        public override void HoldItem(Player player)
        {
            Count = (int)MathHelper.Clamp(Count, 0, 40);
            if (Count >= 40 && !Ready)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(SoundID.Item117, player.position);
                Charged = true;
                Ready = true;
            }
        }
        public override bool CanConsumeAmmo(Item ammo, Player player) => player.ItemUsesThisAnimation != 0;
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<DoubleRifle>())
                .AddIngredient(ItemType<CorruptedXenomite>(), 4)
                .AddIngredient(ItemType<CarbonMyofibre>(), 3)
                .AddIngredient(ItemType<Plating>(), 2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        public override bool AltFunctionUse(Player player)
        {
            if (Charged && player.GetModPlayer<EnergyPlayer>().statEnergy >= 6)
                return true;
            return false;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int laser = 0;
            if (player.altFunctionUse == 2 && Charged)
            {
                laser = 1;
            }
            Projectile.NewProjectile(source, position, velocity, ProjectileType<CorruptedDoubleRifle_Proj>(), damage, knockback, player.whoAmI, laser);
            return false;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }
    }
    public class CorruptedDoubleRifleGlobal : GlobalProjectile //base code from slr, modified
    {
        public override bool InstancePerEntity => true;
        private Item itemSource;
        private bool isHit;
        public bool ShotFrom = false;
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_Parent proj)
            {
                if (proj.Entity is Projectile proj2 && proj2.ModProjectile is CorruptedDoubleRifle_Proj && proj2.owner == Main.myPlayer)
                    itemSource = Main.player[proj2.owner].HeldItem;
            }
        }
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!ShotFrom)
                return;
            if (itemSource != null && itemSource.ModItem is CorruptedDoubleRifle rifle)
                rifle.Count++;
            isHit = true;
        }
        public override void OnKill(Projectile projectile, int timeLeft)
        {
            if (!ShotFrom)
                return;
            if (!isHit && itemSource != null && itemSource.ModItem is CorruptedDoubleRifle rifle)
                rifle.Count--;
        }
    }
}
