using Microsoft.Xna.Framework;
using Redemption.Globals.Player;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.NPCs.Friendly;
using Redemption.NPCs.PreHM;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Globals.NPC
{
    public class GuardNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;
        public int GuardPoints;
        public bool IgnoreArmour;

        public void GuardHit(Terraria.NPC npc, ref double damage, LegacySoundStyle sound, float dmgReduction = 0.25f)
        {
            if (!IgnoreArmour && !npc.HasBuff(BuffID.BrokenArmor) && GuardPoints >= 0)
            {
                damage = (int)(damage * dmgReduction);
                SoundEngine.PlaySound(sound, npc.position);
                GuardPoints -= (int)damage;
            }
        }
        public override void ModifyHitByItem(Terraria.NPC npc, Terraria.Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            if (GuardPoints <= 0)
                return;

            if (item.hammer > 0 || crit)
                IgnoreArmour = true;
        }
        public override void ModifyHitByProjectile(Terraria.NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (GuardPoints <= 0)
                return;

            if (crit || ProjectileTags.Psychic.Has(projectile.type))
                IgnoreArmour = true;
        }
    }
}