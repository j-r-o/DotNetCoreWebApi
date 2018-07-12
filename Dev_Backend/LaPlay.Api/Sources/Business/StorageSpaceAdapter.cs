using System;
using System.Collections.Generic;

using LaPlay.Business.Model;
using LaPlay.Infrastructure.Shell;
using LaPlay.Infrastructure.Repository;

namespace LaPlay.Business
{
    public class StorageSpaceAdapter : IStorageSpaceContract
    {
        private readonly IRepositoryContract _Repository;

        public StorageSpaceAdapter(IRepositoryContract repository)
        {
            _Repository = repository;
        }

        public void CreateStorageSpace(StorageSpace storageSpace)
        {
            _Repository.CreateStorageSpace(storageSpace);
        }
        public StorageSpace ReadStorageSpace(Guid id)
        {
            return _Repository.ReadStorageSpace(id);
        }
        public List<StorageSpace> ReadStorageSpaces()
        {
            return _Repository.ReadStorageSpaces();
        }
        public void UpdateStorageSpace(StorageSpace storageSpace)
        {
            _Repository.UpdateStorageSpace(storageSpace);
        }
        public void DeleteStorageSpace(Guid id)
        {
            _Repository.DeleteStorageSpaces(id);
        }
    }
}