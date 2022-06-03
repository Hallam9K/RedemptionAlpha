using Microsoft.Xna.Framework;
using Redemption.NPCs.Bosses.Cleaver;
using Redemption.NPCs.Bosses.Obliterator;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public class ArenaWorld : ModSystem
    {
        public static bool arenaActive = false;
        public static string arenaBoss = "";
        public static int soloPlayer = -1;
        public static Vector2 arenaTopLeft = new(-1, -1);
        public static Vector2 arenaSize = new(0, 0);
        public static Vector2 arenaMiddle = new(0, 0);

        public override void PostUpdateWorld()
        {
            if (!arenaActive) return;

            switch (arenaBoss)
            {
                case "OO":
                    if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<OO>()))
                    {
                        DeactivateArena();
                        return;
                    }
                    break;
                case "OC":
                    if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<OmegaCleaver>()))
                    {
                        DeactivateArena();
                        return;
                    }
                    break;
                default:
                    DeactivateArena();
                    break;
            }

        }
        private static void DeactivateArena()
        {
            arenaActive = false;
            arenaBoss = "";
            soloPlayer = -1;
            arenaTopLeft = new Vector2(-1, -1);
            arenaSize = new Vector2(0, 0);
            arenaMiddle = new Vector2(0, 0);
        }
    }
    public class ArenaPlayer : ModPlayer
    {
        public override void PostUpdate()
        {
            if (ArenaWorld.arenaActive && ArenaWorld.arenaTopLeft != new Vector2(-1, -1))
            {
                if (ArenaWorld.soloPlayer == -1 || ArenaWorld.soloPlayer == Player.whoAmI)
                {
                    if (Player.Center.X > ArenaWorld.arenaTopLeft.X + ArenaWorld.arenaSize.X)
                    {
                        Vector2 newPos = Player.Center;
                        newPos.X = ArenaWorld.arenaTopLeft.X + ArenaWorld.arenaSize.X;
                        Player.Center = newPos;
                        Player.velocity.X = 0f;
                    }
                    if (Player.Center.X < ArenaWorld.arenaTopLeft.X)
                    {
                        Vector2 newPos = Player.Center;
                        newPos.X = ArenaWorld.arenaTopLeft.X;
                        Player.Center = newPos;
                        Player.velocity.X = 0f;
                    }
                    if (Player.Center.Y > ArenaWorld.arenaTopLeft.Y + ArenaWorld.arenaSize.Y)
                    {
                        Vector2 newPos = Player.Center;
                        newPos.Y = ArenaWorld.arenaTopLeft.Y + ArenaWorld.arenaSize.Y;
                        Player.Center = newPos;
                        Player.velocity.Y = 0f;
                    }
                    if (Player.Center.Y < ArenaWorld.arenaTopLeft.Y)
                    {
                        Vector2 newPos = Player.Center;
                        newPos.Y = ArenaWorld.arenaTopLeft.Y;
                        Player.Center = newPos;
                        Player.velocity.Y = 0.1f;
                    }
                }
                else if (ArenaWorld.soloPlayer != -1 && ArenaWorld.soloPlayer != Player.whoAmI)
                {
                    if (Player.Center.X < ArenaWorld.arenaMiddle.X && Player.Center.X > ArenaWorld.arenaTopLeft.X && Player.Center.Y < ArenaWorld.arenaMiddle.Y)
                    {
                        Vector2 newPos = Player.Center;
                        newPos.X = ArenaWorld.arenaTopLeft.X;
                        Player.Center = newPos;
                        Player.velocity.X = 0f;
                    }
                    if (Player.Center.X > ArenaWorld.arenaMiddle.X && Player.Center.X < ArenaWorld.arenaTopLeft.X + ArenaWorld.arenaSize.X && Player.Center.Y < ArenaWorld.arenaMiddle.Y)
                    {
                        Vector2 newPos = Player.Center;
                        newPos.X = ArenaWorld.arenaTopLeft.X;
                        Player.Center = newPos;
                        Player.velocity.X = 0f;
                    }
                }
            }
        }
    }
}
