using ApiApplication.Core.Application.Features.Commands.CreateShowtime;
using ApiApplication.Core.Application.Models.ViewModels;
using ApiApplication.Core.Application.Repositories;
using ApiApplication.Core.Application.Services;
using ApiApplication.Core.Domain.Entities;
using ApiApplication.Exceptions;
using AutoMapper;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiApplication.Tests.HandlerTests
{
    public class CreateShowtimeCommandHandlerTests
    {
        private CreateShowtimeCommandHandler sut;
        private Mock<IShowtimesRepository> mockShowtimeRepository;
        private Mock<IMapper> mockMapper;
        private Mock<IMovieServiceWrapper> mockMovieServiceWrapper;

        [SetUp]
        public void Setup()
        {
            mockMovieServiceWrapper = new();
            mockShowtimeRepository = new();
            mockMapper = new();

            sut = new(mockShowtimeRepository.Object, mockMovieServiceWrapper.Object, mockMapper.Object);
        }

        [Test]
        public void Handle_WhenMovieNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var command = new CreateShowtimeCommand();
            mockMovieServiceWrapper.Setup(x => x.GetMovieById(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            // Act
            Func<Task> act = async () => await sut.Handle(command, default);

            // Assert
            act.Should().ThrowAsync<NotFoundException>();
        }

        [Test]
        public void Handle_WhenShowtimeIdIsZero_ShouldThrowNotCreatedException()
        {
            // Arrange
            var movieViewModel = new MovieWrapperViewModel();
            var command = new CreateShowtimeCommand();
            var showTimeEntity = new ShowtimeEntity() { Id = 0 };

            mockMovieServiceWrapper.Setup(x => x.GetMovieById(It.IsAny<string>()))
                .ReturnsAsync(() => movieViewModel);

            mockShowtimeRepository.Setup(i => i.CreateShowtime(It.IsAny<ShowtimeEntity>(),
                                                               It.IsAny<CancellationToken>()))
                .ReturnsAsync(showTimeEntity);

            // Act
            Func<Task> act = async () => await sut.Handle(command, default);

            // Assert
            act.Should().ThrowAsync<NotCreatedException>();
        }

        [Test]
        public async Task Handle_WhenEverythingIsValid_ShouldPass()
        {
            // Arrange
            var movieViewModel = new MovieWrapperViewModel();
            var command = new CreateShowtimeCommand();
            var showTimeEntity = new ShowtimeEntity() { Id = 1 };

            mockMovieServiceWrapper.Setup(x => x.GetMovieById(It.IsAny<string>()))
                .ReturnsAsync(() => movieViewModel);

            mockShowtimeRepository.Setup(i => i.CreateShowtime(It.IsAny<ShowtimeEntity>(),
                                                               It.IsAny<CancellationToken>()))
                .ReturnsAsync(showTimeEntity);

            // Act
            var result = await sut.Handle(command, default);

            // Assert
            result.Should().Be(showTimeEntity.Id);
        }
    }
}