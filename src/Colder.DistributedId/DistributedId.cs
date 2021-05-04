using System;

namespace Colder.DistributedId
{
    internal class DistributedId: IDistributedId
    {
        private readonly SequentialGuidType _sequentialGuidType;
        public DistributedId(SequentialGuidType sequentialGuidType = SequentialGuidType.AtEnd)
        {
            _sequentialGuidType = sequentialGuidType;
        }

        public Guid NewGuid()
        {
            return GuidHelper.NewGuid(_sequentialGuidType);
        }
    }
}
