using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using UserDirectoryAPI.API.Controllers;
using UserDirectoryAPI.Application.DTOs;
using UserDirectoryAPI.Application.Interfaces;
using UserDirectoryAPI.Domain.Entities;
using Xunit;

namespace UserDirectoryAPI.Tests;

public class UsersControllerTests
{
    private readonly Mock<IUserRepository> _repoMock = new();
    private readonly Mock<ILogger<UsersController>> _loggerMock = new();

    private UsersController CreateController() =>
        new UsersController(_repoMock.Object, _loggerMock.Object);

    [Fact]
    public async Task GetAll_ReturnsOkWithUsers()
    {
        var users = new List<User>
        {
            new() { Id = Guid.NewGuid(), Name = "Alice", Age = 30, City = "X", State = "Y", Pincode = "1234" }
        };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        var controller = CreateController();

        var result = await controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(users, ok.Value);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((User?)null);

        var controller = CreateController();

        var result = await controller.GetById(id);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains(id.ToString(), notFound.Value!.ToString()!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenFound()
    {
        var user = new User { Id = Guid.NewGuid(), Name = "Bob", Age = 25, City = "C", State = "S", Pincode = "5678" };
        _repoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

        var controller = CreateController();

        var result = await controller.GetById(user.Id);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(user, ok.Value);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenModelInvalid()
    {
        var controller = CreateController();
        controller.ModelState.AddModelError("Name", "Required");

        var dto = new CreateUserDto { Name = "", Age = 0, City = "", State = "", Pincode = "" };

        var result = await controller.Create(dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenValid()
    {
        var dto = new CreateUserDto { Name = "New", Age = 20, City = "Ct", State = "St", Pincode = "9999" };
        User? addedUser = null;
        _repoMock.Setup(r => r.AddAsync(It.IsAny<User>()))
                 .ReturnsAsync((User u) => { addedUser = u; return u; });

        var controller = CreateController();

        var result = await controller.Create(dto);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(UsersController.GetById), created.ActionName);
        Assert.NotNull(created.RouteValues);
        Assert.Equal(addedUser!.Id, created.RouteValues!["id"]);
        var value = Assert.IsType<UserDto>(created.Value);
        Assert.Equal(addedUser.Name, value.Name);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenMissing()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((User?)null);

        var controller = CreateController();

        var dto = new CreateUserDto { Name = "X", Age = 1, City = "C", State = "S", Pincode = "1111" };
        var result = await controller.Update(id, dto);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsNoContent_WhenSuccessful()
    {
        var id = Guid.NewGuid();
        var existing = new User { Id = id, Name = "Old", Age = 10, City = "O", State = "O", Pincode = "0000" };
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask).Verifiable();

        var controller = CreateController();

        var dto = new CreateUserDto { Name = "Updated", Age = 11, City = "N", State = "N", Pincode = "2222" };
        var result = await controller.Update(id, dto);

        Assert.IsType<NoContentResult>(result);
        _repoMock.Verify(r => r.UpdateAsync(It.Is<User>(u => u.Id == id && u.Name == "Updated")), Times.Once);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenMissing()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((User?)null);

        var controller = CreateController();

        var result = await controller.Delete(id);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenSuccessful()
    {
        var id = Guid.NewGuid();
        var existing = new User { Id = id, Name = "ToDelete", Age = 50, City = "X", State = "Y", Pincode = "3333" };
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
        _repoMock.Setup(r => r.DeleteAsync(existing)).Returns(Task.CompletedTask).Verifiable();

        var controller = CreateController();

        var result = await controller.Delete(id);

        Assert.IsType<NoContentResult>(result);
        _repoMock.Verify(r => r.DeleteAsync(existing), Times.Once);
    }
}