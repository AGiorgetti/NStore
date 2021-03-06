﻿using System;
using NStore.Aggregates;
using Xunit;

namespace NStore.Tests.AggregatesTests
{
    public class AggregateTests
    {
        [Fact]
        public void new_aggregate_should_not_be_itialized()
        {
            var ticket = new Ticket();

            Assert.False(ticket.IsInitialized);
            Assert.Equal(0, ticket.Version);
            Assert.False(ticket.IsDirty);
            Assert.Null(ticket.ExposedStateForTest);
        }

        [Fact]
        public void calling_init_more_than_once_should_throw_()
        {
            var ticket = new Ticket();
            ticket.Init("abc");

            var ex = Assert.Throws<AggregateAlreadyInitializedException>(() => ticket.Init("bce"));
            Assert.Equal("abc", ex.AggregateId);
            Assert.Equal(typeof(Ticket), ex.AggregateType);
        }

        [Theory()]
        [InlineData(null)]
        [InlineData("")]
        public void cannot_init_with_invalid_id(string id)
        {
            var ticket = new Ticket();
            Assert.Throws<ArgumentNullException>(() => ticket.Init(id));
        }

        [Fact]
        public void init_without_params_should_create_default_state()
        {
            var ticket = new Ticket();
            ticket.Init("new_ticket");

            Assert.NotNull(ticket.ExposedStateForTest);
            Assert.Equal("new_ticket", ticket.Id);
        }

        [Fact]
        public void append_should_increase_version()
        {
            Ticket ticket = TicketTestFactory.ForTest();
            var persister = (IAggregatePersister) ticket;
            var changeSet = new Changeset(1, new TicketSold());
            persister.ApplyChanges(changeSet);

            Assert.True(ticket.IsInitialized);
            Assert.Equal(1, ticket.Version);
            Assert.False(ticket.IsDirty);
        }

        [Fact]
        public void raising_event_should_not_increase_version()
        {
            Ticket ticket = TicketTestFactory.ForTest();

            ticket.Sale();

            var changes = ticket.ExposePendingChanges();

            Assert.Equal(0, ticket.Version);
            Assert.True(ticket.IsDirty);
            Assert.IsType<TicketSold>(changes.Events[0]);
            Assert.True(ticket.ExposedStateForTest.HasBeenSold);
        }

        [Fact]
        public void aggregate_without_changes_should_build_an_empty_changeset()
        {
            var ticket = TicketTestFactory.ForTest();
            var persister = (IAggregatePersister)ticket;
            var changeSet = persister.GetChangeSet();

            Assert.NotNull(changeSet);
            Assert.True(changeSet.IsEmpty);
        }

        [Fact]
        public void persister_should_create_changeset_with_new_events()
        {
            var ticket = TicketTestFactory.Sold();
            var persister = (IAggregatePersister)ticket;
            var changeSet = persister.GetChangeSet();

            Assert.NotNull(changeSet);
            Assert.False(changeSet.IsEmpty);
            Assert.Equal(1, changeSet.Version);
        }

        [Fact]
        public void persister_should_create_changeset_only_with_new_events()
        {
            var ticket = TicketTestFactory.ForTest();
            var persister = (IAggregatePersister)ticket;

            var changeSet = new Changeset(1, new TicketSold());
            persister.ApplyChanges(changeSet);

            ticket.Refund();

            changeSet = persister.GetChangeSet();

            Assert.NotNull(changeSet);
            Assert.False(changeSet.IsEmpty);
            Assert.Equal(2, changeSet.Version);
            Assert.Equal(1, changeSet.Events.Length);
            Assert.IsType<TicketRefunded>(changeSet.Events[0]);
        }
    }
}