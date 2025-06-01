using ParticleLibrary.Core;
using ParticleLibrary.Core.V3.Particles;
using ParticleLibrary.Core.V3;
using ParticleLibrary.Core.V3.Interfaces;
using System.Collections.Generic;

namespace Redemption.Particles
{
    /// <summary>
    /// A container providing an easy way to store multiple buffers
    /// under the same collection.
    /// </summary>
    public class ParticleCollection
    {
        /// <summary>
        /// A readonly list of the contained buffers.
        /// </summary>
        public IReadOnlyList<ICreatable<ParticleInfo>> ParticleBuffers
        {
            get => _particleBuffers.AsReadOnly();
        }

        private readonly List<ICreatable<ParticleInfo>> _particleBuffers;

        /// <summary>
        /// Creates a new instance of <see cref="ParticleCollection"/>.
        /// </summary>
        public ParticleCollection()
        {
            _particleBuffers = [];
        }

        /// <summary>
        /// Adds a buffer of type <typeparamref name="TBuffer"/> to the collection and registers it to <paramref name="layer"/>.
        /// </summary>
        /// <param name="buffer">The buffer to add to the collection.</param>
        /// <param name="layer">The layer to register to.</param>
        /// <returns>This collection.</returns>
        public ParticleCollection Add<TBuffer>(TBuffer buffer, Layer layer)
            where TBuffer : IUpdatable, IRenderable, ICreatable<ParticleInfo>
        {
            _particleBuffers.Add(buffer);

            ParticleManagerV3.RegisterUpdatable(buffer);
            ParticleManagerV3.RegisterRenderable(layer, buffer);

            return this;
        }

        /// <summary>
        /// Creates a new particle with all contained buffers in <see cref="ParticleBuffers"/> 
        /// with the given <see cref="ParticleInfo"/>.
        /// </summary>
        /// <param name="info">The info.</param>
        public void Create(ParticleInfo info)
        {
            foreach (var buffer in _particleBuffers)
            {
                buffer.Create(info);
            }
        }
        public void Create(params ParticleInfo[] info)
        {
            int i = 0;
            foreach (var buffer in _particleBuffers)
            {
                buffer.Create(info[i]);
                i++;
            }
        }
    }
}