using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlinePictureStorage.Constants
{
    public static class Connections
    {
        public const string blobConnectionString = "DefaultEndpointsProtocol=https;AccountName=onlinepicturestorage;AccountKey=6UmUIOAzkRq1xat+L4r2J18fDYywIn4CVgMFkZt3jb/HBnwS6eGNKFstR8/Tb97OFNwzT96I/SZvjSsjgSGyTA==;EndpointSuffix=core.windows.net";
        public const string blobContainer = "ops-ups";
        public const string blobLink = "https://onlinepicturestorage.blob.core.windows.net/ops-ups/";
        public const string sqlConnectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=ops_db;Integrated Security=True";
    }
}
