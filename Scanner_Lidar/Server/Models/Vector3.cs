using System;
namespace Server_Lidar.Models
{
    public class Vector3 : BaseEntity
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public override string ToString()
        {
            return String.Format("<X:{0},Y:{1},Z:{2}>",X,Y,Z);
        }
    }
}