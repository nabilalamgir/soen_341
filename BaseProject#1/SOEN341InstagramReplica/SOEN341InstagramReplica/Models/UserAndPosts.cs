using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOEN341InstagramReplica.Models
{
    public class UserAndPosts
    {
        public User user { get; set; }
        public IEnumerable<UserPost> posts { get; set; }
        public string following { get; set; }
        public User otherAccount { get; set; }
    }
}