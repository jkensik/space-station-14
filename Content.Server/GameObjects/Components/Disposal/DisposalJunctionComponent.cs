﻿using System.Collections.Generic;
using System.Linq;
using Robust.Shared.GameObjects;
using Robust.Shared.Interfaces.Random;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;

namespace Content.Server.GameObjects.Components.Disposal
{
    [RegisterComponent]
    [ComponentReference(typeof(IDisposalTubeComponent))]
    public class DisposalJunctionComponent : DisposalTubeComponent
    {
#pragma warning disable 649
        [Dependency] private readonly IRobustRandom _random;
#pragma warning restore 649

        /// <summary>
        ///     The angles to connect to.
        /// </summary>
        [ViewVariables]
        private List<Angle> _degrees;

        public override string Name => "DisposalJunction";

        protected override Direction[] ConnectableDirections()
        {
            var direction = Owner.Transform.LocalRotation;

            return _degrees.Select(degree => new Angle(degree.Theta + direction.Theta).GetDir()).ToArray();
        }

        public override Direction NextDirection(DisposalHolderComponent holder)
        {
            var next = Owner.Transform.LocalRotation;
            var directions = ConnectableDirections().Skip(1).ToArray();

            if (Connected.TryGetValue(next.GetDir(), out var forwardTube) &&
                holder.PreviousTube == forwardTube)
            {
                return _random.Pick(directions);
            }

            return next.GetDir();
        }

        public override void ExposeData(ObjectSerializer serializer)
        {
            base.ExposeData(serializer);

            serializer.DataField(ref _degrees, "degrees", null);
        }
    }
}
