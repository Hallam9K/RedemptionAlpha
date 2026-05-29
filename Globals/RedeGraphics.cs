using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Primitives = Redemption.Effects.Trails.Primitives;

namespace Redemption.Globals
{
    public class RedeGraphics : ModSystem
    {
        public static RedeGraphics Instance { get; private set; }
        public RedeGraphics()
        {
            Instance = this;
        }

        private const int INITIAL_SIZE = 1024;

        // Approximately (VertexPositionColorTexture) 21 * INITIAL_SIZE bytes --- 21 kB right now
        // Will double in size each resize (like a List<T>), so it shouldn't be a problem
        public Primitives Primitives { get; private set; }

        public override void Load()
        {
            if (!Main.dedServ)
                Primitives = new Primitives(Main.instance.GraphicsDevice, INITIAL_SIZE, INITIAL_SIZE * 6);
        }

        public override void Unload()
        {
            if (Main.netMode != NetmodeID.Server)
                Main.QueueMainThreadAction(() =>
                {
                    Primitives.Dispose();
                    Primitives = null;
                });
        }
    }
}