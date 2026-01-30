using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cfs.demo.Controllers;
using cfs.demo.Models;
using cfs.demo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace cfs.demo.tests
{
    public class UsersControllerTests
    {
        private readonly Mock<ICfsDatabase> _dbMock = new();
        private readonly Mock<ILogger<UsersController>> _loggerMock = new();

        private UsersController CreateController() =>
            new UsersController(_dbMock.Object, _loggerMock.Object);

        [Fact]
        public async Task GetAll_ReturnsOkWithUsers()
        {
            var users = new List<User>
            {
                new() { Id = Guid.NewGuid(), FirstName = "A", LastName = "Z", Age = 30, City = "X", State = "S", Pincode = "1234" },
                new() { Id = Guid.NewGuid(), FirstName = "B", LastName = "Y", Age = 40, City = "Y", State = "S", Pincode = "5678" }
            };
            _dbMock.Setup(d => d.GetAllAsync()).ReturnsAsync(users);

            var controller = CreateController();
            var result = await controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsAssignableFrom<IEnumerable<User>>(ok.Value);
            Assert.Equal(2, returned.Count());
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMissing()
        {
            _dbMock.Setup(d => d.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

            var controller = CreateController();
            var result = await controller.GetById(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenFound()
        {
            var id = Guid.NewGuid();
            var user = new User { Id = id, FirstName = "F", LastName = "L", Age = 25, City = "C", State = "S", Pincode = "9999" };
            _dbMock.Setup(d => d.GetByIdAsync(id)).ReturnsAsync(user);

            var controller = CreateController();
            var result = await controller.GetById(id);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<User>(ok.Value);
            Assert.Equal(id, returned.Id);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtRoute_WithCreatedUser()
        {
            _dbMock.Setup(d => d.CreateAsync(It.IsAny<User>()))
                   .ReturnsAsync((User u) => u);

            var controller = CreateController();
            var dto = new UserCreateDto
            {
                FirstName = "New",
                LastName = "User",
                Age = 20,
                City = "City",
                State = "State",
                Pincode = "1234"
            };

            var result = await controller.Create(dto);

            var created = Assert.IsType<CreatedAtRouteResult>(result.Result);
            Assert.Equal("GetUserById", created.RouteName);
            var returned = Assert.IsType<User>(created.Value);
            Assert.Equal(dto.FirstName, returned.FirstName);
            Assert.Equal(dto.LastName, returned.LastName);
            Assert.Equal(dto.Age, returned.Age);
            Assert.Equal(dto.City, returned.City);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenUpdated()
        {
            var id = Guid.NewGuid();
            var existing = new User { Id = id, FirstName = "X", LastName = "Y", Age = 50, City = "C", State = "S", Pincode = "0000" };

            _dbMock.Setup(d => d.GetByIdAsync(id)).ReturnsAsync(existing);
            _dbMock.Setup(d => d.UpdateAsync(It.IsAny<User>())).ReturnsAsync(true);

            var controller = CreateController();
            var dto = new UserUpdateDto { Age = 55 };

            var result = await controller.Update(id, dto);

            Assert.IsType<NoContentResult>(result);
            _dbMock.Verify(d => d.UpdateAsync(It.Is<User>(u => u.Id == id && u.Age == 55)), Times.Once);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenMissing()
        {
            _dbMock.Setup(d => d.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

            var controller = CreateController();
            var result = await controller.Update(Guid.NewGuid(), new UserUpdateDto { Age = 10 });

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDeleted()
        {
            var id = Guid.NewGuid();
            _dbMock.Setup(d => d.DeleteAsync(id)).ReturnsAsync(true);

            var controller = CreateController();
            var result = await controller.Delete(id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenMissing()
        {
            _dbMock.Setup(d => d.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            var controller = CreateController();
            var result = await controller.Delete(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result);
        }
    }
}