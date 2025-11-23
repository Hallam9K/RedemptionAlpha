using Redemption.Base;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Microsoft.Xna.Framework;

namespace Redemption.Globals.NPCs
{
    public class HitboxNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override void ResetEffects(Terraria.NPC npc)
        {
            extendedHitbox = Rectangle.Empty;
        }
        public bool IntersectsNPCExtended(Rectangle rect, Terraria.NPC npc) => rect.Intersects(npc.Hitbox) || rect.Intersects(extendedHitbox);
        public Rectangle extendedHitbox;
        public void CreateExtendedHitbox(Rectangle hitbox) => extendedHitbox = hitbox;
        public static bool HitPlayerInHitbox(Terraria.NPC npc, Rectangle rect, int damage, float knockback = 4.5f)
        {
            bool hit = false;
            for (int p = 0; p < Main.maxPlayers; p++)
            {
                Terraria.Player target = Main.player[p];
                if (!target.active || target.dead)
                    continue;

                if (!target.Hitbox.Intersects(rect))
                    continue;

                int hitDirection = target.RightOfDir(npc);

                if (target.whoAmI == Main.myPlayer)
                    BaseAI.DamagePlayer(target, damage, knockback, hitDirection, npc);
                hit = true;
            }
            return hit;
        }
        public void DamageInHitbox(Terraria.NPC npc, Rectangle rect, int damage, float knockback = 4.5f, bool parried = false, int CD = 30)
        {
            CreateExtendedHitbox(rect);
            if (!parried)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    Terraria.NPC target = Main.npc[i];
                    if (!target.active || target.whoAmI == npc.whoAmI)
                        continue;

                    if (npc.Redemption().attacker == null || npc.Redemption().attacker is not Terraria.NPC || npc.Redemption().attacker != target)
                    {
                        if (!target.friendly && !NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[target.type])
                            continue;
                    }

                    if (target.immune[npc.whoAmI] > 0 || !target.Hitbox.Intersects(rect))
                        continue;

                    target.immune[npc.whoAmI] = CD;
                    int hitDirection = target.RightOfDir(npc);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        BaseAI.DamageNPC(target, damage, knockback, hitDirection, npc);
                }
                HitPlayerInHitbox(npc, rect, damage, knockback);
            }
        }
        public bool DamageInHitbox(Terraria.NPC npc, byte ID, Rectangle rect, int damage, float knockback = 4.5f, bool parried = false, int CD = 30, bool hitAnyPlayer = false)
        {
            bool hit = false;

            CreateExtendedHitbox(rect);
            if (!parried)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    Terraria.NPC target = Main.npc[i];
                    if (!target.active || target.whoAmI == npc.whoAmI)
                        continue;

                    bool friendly = target.friendly || target.lifeMax <= 5 || NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[target.type];
                    if (npc.Redemption().attacker == null || npc.Redemption().attacker is not Terraria.NPC || npc.Redemption().attacker != target)
                    {
                        if (friendly)
                            continue;
                    }
                    switch (ID)
                    {
                        case 1:
                            if (NPCLists.Plantlike.Contains(target.type))
                                continue;
                            break;
                        case 2:
                            if (npc.ai[3] >= 0 && npc.ai[3] < Main.maxPlayers && Main.player[(int)npc.ai[3]].RedemptionPlayerBuff().skeletonFriendly && NPCLists.SkeletonHumanoid.Contains(target.type))
                                continue;
                            break;
                    }

                    if (target.immune[npc.whoAmI] > 0 || !target.Hitbox.Intersects(rect))
                        continue;

                    target.immune[npc.whoAmI] = CD;
                    int hitDirection = target.RightOfDir(npc);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        BaseAI.DamageNPC(target, damage, knockback, hitDirection, npc);
                    hit = true;
                }
                if (!hitAnyPlayer)
                {
                    if (npc.Redemption().attacker == null || npc.Redemption().attacker is not Terraria.Player)
                        return hit;
                }
                if (HitPlayerInHitbox(npc, rect, damage, knockback))
                    hit = true;
            }
            return hit;
        }
    }
}