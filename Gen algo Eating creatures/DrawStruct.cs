using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Gen_algo_Eating_creatures
{
    struct DrawStruct
    {
        public Vector3 translation;
        public Vector3 scale;
        public float rotation;

        public DrawStruct(Vector3 trans, Vector3 sca, float rot)
        {
            translation = trans;
            scale = sca;
            rotation = rot;
        }
    }
}
