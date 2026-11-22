using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Entities
{
    /// <summary>
    /// Actions of the entities
    /// </summary>
    public enum Action
    {
        None = 0,
        Idle = 1,
        Walk = 2,
        Run = 3,
        Attack = 4,
        Walk_Attack = 5,
        Run_Attack = 6,
        Hurt = 7,
        Death = 8
    }
}
