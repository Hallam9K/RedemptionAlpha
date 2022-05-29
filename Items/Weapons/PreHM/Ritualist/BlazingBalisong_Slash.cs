using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Redemption.Globals;
using Terraria.DataStructures;
using Redemption.DamageClasses;
using Redemption.Buffs.NPCBuffs;
using Terraria.ID;

namespace Redemption.Items.Weapons.PreHM.Ritualist
{
    public class BlazingBalisong_Slash : Incisor_Slash
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blazing Balisong");
            Main.projFrames[Projectile.type] = 5;
        }
        public override void PostAI()
        {
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            RedeProjectile.Decapitation(target, ref damage, ref crit);
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 260);
        }
    }
}