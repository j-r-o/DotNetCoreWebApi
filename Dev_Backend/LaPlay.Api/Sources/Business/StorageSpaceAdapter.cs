using System;
using System.Collections.Generic;

using LaPlay.Business.Model;
using LaPlay.Infrastructure.Shell;

namespace LaPlay.Business
{
    class StorageSpaceAdapter : IStorageSpaceContract
    {
        private readonly IRepositoryContract _Repository;

        public StorageSpaceAdapter(IRepositoryContract repository)
        {
            _Repository = repository;
        }

        public StorageSpace createStorageSpace(StorageSpace storageSpace)
        {
            return _Repository.
        }
        StorageSpace readStorageSpace(Guid id)
        {

        }
        List<StorageSpace> readStorageSpaces()
        {

        }
        StorageSpace updateStorageSpace(StorageSpace storageSpace)
        {

        }
        void deleteStorageSpaces(Guid id)
        {
            
        }
    }
}