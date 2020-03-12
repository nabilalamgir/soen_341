using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOEN341InstagramReplica.Models
{
    public class PostAndComments
    {
        public string postUserName { get; set; }
        public UserPost post { get; set; }
        public IEnumerable<Comment> comments { get; set; }
        public int likeStatus { get; set; }
    }
}