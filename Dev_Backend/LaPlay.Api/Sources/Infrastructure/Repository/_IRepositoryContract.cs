
using System;
using System.Collections.Generic;

using LaPlay.Business.Model;

namespace LaPlay.Infrastructure.Repository
{
    public interface IRepositoryContract
    {
        void CreateStorageSpace(StorageSpace storageSpace);
        StorageSpace ReadStorageSpace(Guid id);
        List<StorageSpace> ReadStorageSpaces();
        void UpdateStorageSpace(StorageSpace storageSpace);
        void DeleteStorageSpaces(Guid id);
    }
}