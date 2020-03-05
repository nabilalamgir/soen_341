using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOEN341InstagramReplica.Models
{
    public class SubNamesAndLatestPost
    {
        public int subID { get; set; }
        public string subUsername { get; set; }
        public UserPost latestPost { get; set; }
    }
}