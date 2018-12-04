using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TodoApi.Models;
using TodoApi.Services;
using Xunit;

namespace TodoApi.Tests
{
    public class TodoServiceTests
    {
        [Fact]
        public void GetAllLists_should_return_lists()
        {
            // Arrange - We're mocking our dbSet & dbContext

            // in-memory data
            IQueryable<TodoList> lists = new List<TodoList>()
            {
                new TodoList()
                {

                    Id = 1,
                    Name = "First list",
                    Items = new HashSet<TodoItem>(),
                },
                new TodoList()
                {

                    Id = 2,
                    Name = "Second list",
                    Items = new HashSet<TodoItem>(),
                },
            }.AsQueryable();

            // To query our database we need to implement IQueryable 
            var mockSet = new Mock<DbSet<TodoList>>();
            mockSet.As<IQueryable<TodoList>>().Setup(m => m.Provider).Returns(lists.Provider);
            mockSet.As<IQueryable<TodoList>>().Setup(m => m.Expression).Returns(lists.Expression);
            mockSet.As<IQueryable<TodoList>>().Setup(m => m.ElementType).Returns(lists.ElementType);
            mockSet.As<IQueryable<TodoList>>().Setup(m => m.GetEnumerator()).Returns(lists.GetEnumerator());

            var mockContext = new Mock<TodoContext>();
            mockContext.Setup(c => c.Lists).Returns(mockSet.Object);
            
            // We also need a push service
            var mockPush = new Mock<IPushService>();

            // Act
            var sub = new TodoService(null, mockContext.Object, mockPush.Object);
            var actual = sub.GetAllLists();

            // Assert
            actual.Should()
                .NotBeEmpty()
                .And.HaveCount(2);

            actual.First().Value.Should().Be("First list");
        }
    }
}
