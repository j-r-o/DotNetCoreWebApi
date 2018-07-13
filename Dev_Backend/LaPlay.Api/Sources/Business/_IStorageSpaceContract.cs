using System;
using System.Collections.Generic;

using LaPlay.Business.Model;

namespace LaPlay.Business
{
    public interface IStorageSpaceContract
    {
        void CreateStorageSpace(StorageSpace storageSpace);
        StorageSpace ReadStorageSpace(Guid id);
        List<StorageSpace> ReadStorageSpaces();
        void UpdateStorageSpace(StorageSpace storageSpace);
        void DeleteStorageSpace(Guid id);
    }
}