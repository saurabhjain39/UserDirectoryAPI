using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using UserDirectoryAPI.Domain.Entities;
using UserDirectoryAPI.Infrastructure.Data;
using UserDirectoryAPI.Infrastructure.Repositories;
using Xunit;

namespace UserDirectoryAPI.Tests;

public class UserRepositoryTests
{
    private static AppDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_AddsUser()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var logger = new Mock<ILogger<UserRepository>>();
        var repo = new UserRepository(context, logger.Object);

        var user = new User { Id = Guid.NewGuid(), Name = "A", Age = 1, City = "C", State = "S", Pincode = "1111" };
        var result = await repo.AddAsync(user);

        Assert.Equal(user.Id, result.Id);
        Assert.Equal(1, await context.Users.CountAsync());
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAll()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        context.Users.AddRange(
            new User { Id = Guid.NewGuid(), Name = "A", Age = 1, City = "C", State = "S", Pincode = "1" },
            new User { Id = Guid.NewGuid(), Name = "B", Age = 2, City = "C", State = "S", Pincode = "2" }
        );
        await context.SaveChangesAsync();

        var repo = new UserRepository(context, new Mock<ILogger<UserRepository>>().Object);

        var result = await repo.GetAllAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsUser_OrNull()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var user = new User { Id = Guid.NewGuid(), Name = "A", Age = 1, City = "C", State = "S", Pincode = "1" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repo = new UserRepository(context, new Mock<ILogger<UserRepository>>().Object);

        var found = await repo.GetByIdAsync(user.Id);
        Assert.NotNull(found);
        Assert.Equal(user.Id, found!.Id);

        var missing = await repo.GetByIdAsync(Guid.NewGuid());
        Assert.Null(missing);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesUser()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var user = new User { Id = Guid.NewGuid(), Name = "Old", Age = 1, City = "C", State = "S", Pincode = "1" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repo = new UserRepository(context, new Mock<ILogger<UserRepository>>().Object);

        user.Name = "New";
        await repo.UpdateAsync(user);

        var updated = await context.Users.FindAsync(user.Id);
        Assert.Equal("New", updated!.Name);
    }

    [Fact]
    public async Task DeleteAsync_RemovesUser()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var user = new User { Id = Guid.NewGuid(), Name = "X", Age = 1, City = "C", State = "S", Pincode = "1" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repo = new UserRepository(context, new Mock<ILogger<UserRepository>>().Object);

        await repo.DeleteAsync(user);

        var exists = await context.Users.FindAsync(user.Id);
        Assert.Null(exists);
    }
}