using System;
using System.Collections.Generic;

using LaPlay.Business.Model;

namespace LaPlay.Business
{
    public interface IStorageSpaceContract
    {
        StorageSpace createStorageSpace(String name, String mainDirectoryPath, String mirrorDirectoryPath);
        StorageSpace readStorageSpace(Guid id);
        List<StorageSpace> readStorageSpaces();
        StorageSpace updateStorageSpace(StorageSpace storageSpace);
        void deleteStorageSpaces(Guid id);
    }
}