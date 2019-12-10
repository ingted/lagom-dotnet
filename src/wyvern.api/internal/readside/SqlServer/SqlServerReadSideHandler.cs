// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Akka;
using Akka.Configuration;
using Akka.Persistence.Query;
using Akka.Streams.Dsl;
using Dapper;
using Microsoft.Extensions.Configuration;
using wyvern.api.abstractions;
using wyvern.entity.@event.aggregate;

namespace wyvern.api.@internal.readside.SqlServer
{
    // TODO: Continue working on making this internal

    public class SqlServerReadSideHandler<TE> : ReadSideHandler<TE>
        where TE : AggregateEvent<TE>
    {
        private static Func<string, Func<SqlConnection>> ReadSideConnectionFactoryInitializer { get; }
            = (constr)
            => ()
            => new SqlConnection(constr);

        protected Func<SqlConnection> ReadSideConnectionFactory { get; }
        public string ReadSideId { get; }
        public Action<SqlConnection> GlobalPrepareCallback { get; }
        public Action<SqlConnection, AggregateEventTag> PrepareCallback { get; }
        public Dictionary<Type, Action<SqlConnection, EventStreamElement<TE>>> EventHandlers { get; }

        public SqlServerReadSideHandler(
            IConfiguration config,
            Config config2,
            string readSideId,
            Action<SqlConnection> globalPrepareCallback,
            Action<SqlConnection, AggregateEventTag> prepareCallback,
            Dictionary<Type, Action<SqlConnection, EventStreamElement<TE>>> eventHandlers
        )
        {
            ReadSideId = readSideId;
            GlobalPrepareCallback = globalPrepareCallback;
            PrepareCallback = prepareCallback;
            EventHandlers = eventHandlers;

            var constr = config.GetConnectionString(ReadSideId);
            if (String.IsNullOrEmpty(constr))
                constr = config2.GetString("db.default.connection-string");
            ReadSideConnectionFactory = ReadSideConnectionFactoryInitializer(constr);
        }

        public override Task<Done> GlobalPrepare()
        {
            using (var con = ReadSideConnectionFactory.Invoke())
            {
                con.Execute(@"
            if not exists (select * from sysobjects where name='read_side_offsets' and xtype='U')
                CREATE TABLE read_side_offsets (read_side_id VARCHAR(255), tag VARCHAR(255),sequence_offset bigint, time_uuid_offset char(36),PRIMARY KEY (read_side_id, tag))
            ");
                GlobalPrepareCallback(con);
            }
            return base.GlobalPrepare();
        }

        public override Task<Offset> Prepare(AggregateEventTag tag)
        {
            using (var con = ReadSideConnectionFactory.Invoke())
            {
                var offset = con.QueryFirstOrDefault<long>(@"
                select sequence_offset
                from read_side_offsets
                where read_side_id = @readSideId
                and tag = @tag
            ", new
                {
                    readSideId = ReadSideId,
                    tag = tag.Tag
                });
                return Task.FromResult(
                    Offset.Sequence(
                        offset
                    )
                );
            }
        }

        public virtual void DbActionExecutor(EventStreamElement<TE> element, Action<SqlConnection, EventStreamElement<TE>> action)
        {
            try
            {
                using (var con = ReadSideConnectionFactory.Invoke())
                    action(con, element);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override Flow<EventStreamElement<TE>, Done, NotUsed> Handle()
        {
            return Flow.FromFunction(
                new Func<EventStreamElement<TE>, Done>(
                    element =>
                    {
                        if (EventHandlers.TryGetValue(element.Event.GetType(), out var dbAction))
                        {
                            DbActionExecutor(element, dbAction);
                        }
                        else
                        {
                            // TODO: log the unhandled event
                        }

                        using (var con = ReadSideConnectionFactory.Invoke())
                        {
                            con.Execute(@"
                                update read_side_offsets
                                set sequence_offset = @offset
                                where read_side_id = @readSideId
                                and tag = @tag",
                                new
                                {
                                    offset = (element.Offset as Sequence).Value,
                                    readSideId = ReadSideId,
                                    tag = (element.Event.AggregateTag as AggregateEventTag).Tag
                                });
                        }

                        return Done.Instance;
                    }
                )
            );
        }

    }
}