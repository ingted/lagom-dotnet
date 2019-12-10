// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


// using System;
// using System.Data.SqlClient;
// using Microsoft.Extensions.Configuration;

// public class DbConnectionFactory
// {
//     readonly string constrKey;
//     readonly Func<SqlConnection> factory;
//     public DbConnectionFactory(string constrKey, IConfiguration config) =>
//         factory = () =>
//             new SqlConnection(config.GetConnectionString(constrKey));
//     public SqlConnection Create() => factory();
// }



