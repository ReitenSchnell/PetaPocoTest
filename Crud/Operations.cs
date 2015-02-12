using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using FluentAssertions;
using PetaPoco;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace Crud
{
    public class Operations
    {
        public List<Feed> GetAllFeeds()
        {
            return CreateContext().Query<Feed>("Select * from Feeds").ToList();
        }

        public Feed GetFeedById(Guid id)
        {
            return CreateContext().SingleOrDefault<Feed>("Select * from Feeds where FeedId=@0", id);
        }

        public void CreateFeed(Feed feed)
        {
            CreateContext().Insert(feed);
        }

        public void UpdateFeed(Feed feed)
        {
            CreateContext().Update(feed, feed.FeedId);
        }

        public void DeleteFeed(Feed feed)
        {
            CreateContext().Delete(feed);
        }

        private static Database CreateContext()
        {
            return new Database("Data");
        }
    }

    public class OperationsIntegrationTests
    {
        private readonly Operations operations = new Operations();
        
        [Theory, AutoData]
        public void should_create_and_read_all_feeds(List<Feed> feeds)
        {
            feeds.ForEach(feed => operations.CreateFeed(feed));
            var result = operations.GetAllFeeds();
            result.Count.Should().BeGreaterThan(0);
        }

        [Theory, AutoData]
        public void should_find_feed_by_id(List<Feed> feeds)
        {
            feeds.ForEach(feed => operations.CreateFeed(feed));
            var result = operations.GetFeedById(feeds[1].FeedId);
            result.ShouldBeEquivalentTo(feeds[1]);
        }

        [Theory, AutoData]
        public void should_update_feed(List<Feed> feeds, string address)
        {
            feeds.ForEach(feed => operations.CreateFeed(feed));
            feeds[1].Address = address;
            operations.UpdateFeed(feeds[1]);
            operations.GetFeedById(feeds[1].FeedId).Address.Should().Be(address);
        }

        [Theory, AutoData]
        public void should_return_null_if_feed_not_found()
        {
            var result = operations.GetFeedById(new Guid());
            result.Should().BeNull();
        }

        [Theory, AutoData]
        public void should_remove_feed(List<Feed> feeds)
        {
            feeds.ForEach(feed => operations.CreateFeed(feed));
            var middleFeed = feeds[1];
            operations.DeleteFeed(middleFeed);
            var afterRemove = operations.GetFeedById(middleFeed.FeedId);
            afterRemove.Should().BeNull();
        }
    }
}
