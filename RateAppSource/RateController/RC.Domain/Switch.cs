using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.Domain
{
    public class Switch
    {
        public PGN32618 SwitchBox;
        public int Id { get; set; }
        public Switch() 
        {
            SwitchBox = new PGN32618();
        }


    }
}
