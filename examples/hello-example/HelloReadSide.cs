using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using wyvern.api.abstractions;
using wyvern.api.@internal.readside;
using wyvern.api.@internal.readside.SqlServer;
using wyvern.entity.@event.aggregate;

public class HelloReadSideProcessor : ReadSideProcessor<HelloEvent>
{
    public override AggregateEventTag[] AggregateTags => new AggregateEventTag[] { HelloEvent.Tag };

    Dictionary<Type, Action<SqlConnection, EventStreamElement<HelloEvent>>> Handlers { get; }

    public HelloReadSideProcessor()
    {
        Handlers = new Dictionary<Type, Action<SqlConnection, EventStreamElement<HelloEvent>>>();
        Handlers.Add(typeof(HelloEvent.GreetingUpdatedEvent), HandleChangeGreeting);
    }

    void HandleChangeGreeting(SqlConnection con, EventStreamElement<HelloEvent> eventBase)
    {
        var e = eventBase.Event as HelloEvent.GreetingUpdatedEvent;

        try
        {
            con.Execute(@"
            update greetings set message=@message where name=@name
            IF @@ROWCOUNT=0
            insert into greetings(message, name) values(@message, @name);",
                new
                {
                    name = eventBase.EntityId,
                    message = e.Message
                }
            );
        }
        catch (Exception ex)
        {
            // TODO: Catch this from the parent caller
            Console.WriteLine(ex.Message);
            throw;
        }

    }

    public override ReadSideHandler<HelloEvent> BuildHandler() =>
        new SqlServerReadSideHandler<HelloEvent>(
            Config,
            Config2,
            typeof(HelloReadSideProcessor).Name,
            GlobalPrepare,
            (con, tag) => { /* noop */},
            Handlers
        );

    void GlobalPrepare(SqlConnection con)
    {
        foreach (var tag in AggregateTags)
            con.Execute(@"
                if not exists (select * from read_side_offsets
                        where read_side_id = @readSideId
                        and tag = @tag)
                    insert into read_side_offsets
                    (sequence_offset, read_side_id, tag)
                    values
                    (0, @readSideId, @tag)
                ",
                new
                {
                    readSideId = ReadSideName,
                    tag = tag.Tag
                }
            );

        con.Execute(@"
            if not exists (select * from sys.tables where name = 'Greetings')
                create table Greetings (
                    name varchar(128) not null,
                    message varchar(128) not null,
                    timestamp datetime not null default getdate(),
                    primary key (name)
                );
        ");
    }

}