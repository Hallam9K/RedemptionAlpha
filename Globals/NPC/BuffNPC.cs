using Microsoft.Xna.Framework;
using Redemption.NPCs.Critters;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption
{
    public class BuffNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;

        public bool infested;
        public int infestedTime;

        public override void ResetEffects(NPC npc)
        {
            infested = false;
            infestedTime = 0;
        }
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (infested)
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                npc.lifeRegen -= infestedTime / 60;
                npc.defense -= infestedTime / 120;
            }
        }
        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (infested)
                drawColor = Color.GreenYellow * 0.4f;
        }
        public override bool PreKill(NPC npc)
        {
            if (infested && infestedTime >= 60)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath19, npc.position);
                for (int i = 0; i < 20; i++)
                {
                    int dustIndex4 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.GreenBlood, Scale: 3f);
                    Main.dust[dustIndex4].velocity *= 5f;
                }
                int larvaCount = infestedTime / 120 + 1;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < MathHelper.Clamp(larvaCount, 1, 8); i++)
                        Projectile.NewProjectile(npc.GetProjectileSpawnSource(), npc.Center, RedeHelper.Spread(6), ModContent.ProjectileType<GrandLarvaFall>(), 0, 0, Main.myPlayer);
                }
            }
            return true;
        }
    }
}