using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.RouteAnalyzer;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing.Internal;
using Xunit;
using Xunit.Sdk;

namespace wyvern.api.tests.ioc
{
    public class UserWithId
    {
        public string Id { get; set; }
        public string Name { get; set;  }
    }

    public class User
    {
        public string Name { get; set; }
    }

    public class UserFriend
    {
        public string Id { get; set;  }
        public string Name { get; set; }
    }



}
