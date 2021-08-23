using Microsoft.Xna.Framework;
using Redemption.Buffs;
using Redemption.Buffs.Debuffs;
using Redemption.NPCs.Critters;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Globals.NPC
{
    public class BuffNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;

        public bool infested;
        public bool devilScented;
        public int infestedTime;
        public bool rallied;

        public override void ResetEffects(Terraria.NPC npc)
        {        
            devilScented = false;
            rallied = false;
            if (!npc.HasBuff(ModContent.BuffType<InfestedDebuff>()))
            {
                infested = false;
                infestedTime = 0;
            }
        }
        public override void UpdateLifeRegen(Terraria.NPC npc, ref int damage)
        {
            if (infested)
            {
                infestedTime++;
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                npc.lifeRegen -= infestedTime / 120;
            }
        }
        public override bool StrikeNPC(Terraria.NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (infested)
            {
                if (npc.defense > 0)
                    npc.defense -= infestedTime / 120;
            }
            if (rallied)
                damage *= 0.85;
            return true;
        }
        public override void ModifyHitPlayer(Terraria.NPC npc, Terraria.Player target, ref int damage, ref bool crit)
        {
            if (rallied)
                damage = (int)(damage * 1.15f);
        }
        public override void ModifyHitNPC(Terraria.NPC npc, Terraria.NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (rallied)
                damage = (int)(damage * 1.15f);
        }
        public override void DrawEffects(Terraria.NPC npc, ref Color drawColor)
        {
            if (infested)
                drawColor = new Color(197, 219, 171);
            if (rallied)
                drawColor = new Color(200, 150, 150);
        }
        public override bool PreKill(Terraria.NPC npc)
        {
            if (infested && infestedTime >= 60 && npc.lifeMax > 5)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath19, npc.position);
                for (int i = 0; i < 20; i++)
                {
                    int dustIndex4 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.GreenBlood, Scale: 3f);
                    Main.dust[dustIndex4].velocity *= 5f;
                }
                int larvaCount = infestedTime / 180 + 1;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < MathHelper.Clamp(larvaCount, 1, 8); i++)
                        Projectile.NewProjectile(npc.GetProjectileSpawnSource(), npc.Center, RedeHelper.SpreadUp(8), ModContent.ProjectileType<GrandLarvaFall>(), 0, 0, Main.myPlayer);
                }
            }
            return true;
        }
    }
}