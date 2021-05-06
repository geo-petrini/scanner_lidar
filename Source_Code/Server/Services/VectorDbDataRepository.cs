using Server_Lidar.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server_Lidar.Services
{
    public class VectorDbDataRepository : DbDataRepository<AppDbContext, Vector3>, IVector3Repository
    {
        public VectorDbDataRepository(AppDbContext ctx) : base(ctx)
        {
        }
    }
}
