using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria.ModLoader;

namespace Redemption.Tags
{
    public sealed class ProjectileTags : TagGroup
    {
        public override int TypeCount => ProjectileLoader.ProjectileCount;
    }
}