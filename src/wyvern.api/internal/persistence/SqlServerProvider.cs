// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;
using System.Data.SqlClient;
using Akka.Configuration;

namespace wyvern.api.@internal.persistence
{
    /// <summary>
    /// Provides a connection factory to the default database instance
    /// </summary>
    internal class SqlServerProvider
    {
        /// <summary>
        /// subsection configuration
        /// </summary>
        /// <value></value>
        Config Config { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="config"></param>
        public SqlServerProvider(Config config)
        {
            Config = config.GetConfig("db.default");
        }

        /// <summary>
        /// Provides a SqlConnection factory
        /// </summary>
        /// <returns></returns>
        public Func<SqlConnection> GetconnectionProvider()
        {
            var connectionString = Config.GetString("connection-string");
            return () =>
            {
                var con = new SqlConnection(connectionString);
                con.Open();
                return con;
            };
        }
    }
}
