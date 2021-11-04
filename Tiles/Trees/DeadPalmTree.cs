using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Placeable.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Trees
{
    class DeadPalmTree : ModPalmTree
    {
        private static Mod Mod
        {
            get
            {
                return ModLoader.GetMod("Redemption");
            }
        }

        public override int CreateDust()
        {
            return DustID.Ash;
        }

        public override int DropWood()
        {
            return ModContent.ItemType<PetrifiedWood>();
        }

        public override Texture2D GetTexture()
        {
            return Mod.Assets.Request<Texture2D>("Tiles/Trees/DeadPalmTree").Value;
        }

        public override Texture2D GetTopTextures()
        {
            return Mod.Assets.Request<Texture2D>("Tiles/Trees/DeadPalmTree_Tops").Value;
        }
    }
}
