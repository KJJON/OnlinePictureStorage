using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlinePictureStorage.Constants
{
    public static class Connections
    {
        public const string blobConnectionString = "DefaultEndpointsProtocol=https;AccountName=sqlvayljnkyyyv2fm2;AccountKey=kTfCbLOY+eT2l4l5DdL3c8QaXvq9t58gRzVOLGRmn3h0fNwzK5HIB76mGP414VH9SrdWBRhUucPuD7YbCNpY5Q==;EndpointSuffix=core.windows.net";
        public const string blobContainer = "ops-picturestorage";
        public const string sqlConnectionString = "Data Source=onlinepicturestorage.database.windows.net;Initial Catalog = OPSDB; Persist Security Info=True;User ID = OPSAdmin; Password=095DBA//Pass";
    }
}
