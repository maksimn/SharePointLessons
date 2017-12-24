using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunchVoting.Classes {
    public class LunchVoteData {
        public string Selection { get; set; }
        public int VoteCount { get; set; } // количество голосов за этот выбор
        public int OnBehalfOfCount { get; set; } // количество голосов от чужого лица
        public bool YouVoted { get; set; }
        public bool YouVotedByProxy { get; set; }
    }
}
