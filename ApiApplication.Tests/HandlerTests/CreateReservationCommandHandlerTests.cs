using ApiApplication.Core.Application.Features.Commands.CreateReservation;
using ApiApplication.Core.Application.Models.RequestModels;
using ApiApplication.Core.Application.Models.ViewModels;
using ApiApplication.Core.Application.Repositories;
using ApiApplication.Core.Domain.Entities;
using ApiApplication.Exceptions;
using AutoMapper;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace ApiApplication.Tests.HandlerTests
{
    internal class CreateReservationCommandHandlerTests
    {
        private CreateReservationCommandHandler sut;
        private Mock<IShowtimesRepository> mockShowtimeRepository;
        private Mock<ITicketsRepository> mockTicketsRepository;
        private Mock<IMapper> mockMapper;

        [SetUp]
        public void Setup()
        {
            mockShowtimeRepository = new();
            mockTicketsRepository = new();
            mockMapper = new();

            sut = new CreateReservationCommandHandler(mockTicketsRepository.Object, mockMapper.Object, mockShowtimeRepository.Object);
        }

        [Test]
        public async Task Handle_WhenSeatsAreNotAdjacent_ShouldThrowContigiousSeatsException()
        {
            //Arrange
            var command = GetCreateReservationCommand(false);

            // Action
            Func<Task> action = async () => await sut.Handle(command, default);

            // Assert
            await action.Should().ThrowAsync<ContigiousSeatsException>();
        }


        [Test]
        public async Task Handle_WhenShowtimeIsNull_ShouldThrowNotFoundException()
        {
            //Arrange
            var command = GetCreateReservationCommand();
            mockShowtimeRepository.Setup(i => i.GetAllAsync(It.IsAny<Expression<Func<ShowtimeEntity, bool>>>(),
                                                            It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => null);

            // Action
            Func<Task> action = () => sut.Handle(command, default);

            // Assert
            await action.Should().ThrowAsync<NotFoundException>();
        }


        [Test]
        public async Task Handle_WhenTicketIsAlreadyPaid_ShouldThrowReservationException()
        {
            //Arrange
            var command = GetCreateReservationCommand();
            var showTimeEntity = new ShowtimeEntity();
            var tickets = new List<TicketEntity>()
        {
            new()
            {
                Paid = true,
                CreatedTime = DateTime.Now.AddMinutes(-20),
                TicketSeats = new List<TicketSeatsEntity>()
            }
        };

            mockShowtimeRepository.Setup(i => i.GetAllAsync(It.IsAny<Expression<Func<ShowtimeEntity, bool>>>(),
                                                            It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new[] { showTimeEntity });

            mockTicketsRepository.Setup(i => i.GetTicketsBySeats(It.IsAny<int>(),
                                                                 It.IsAny<List<SeatRequestModel>>(),
                                                                 It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => tickets);

            // Action
            Func<Task> action = async () => await sut.Handle(command, default);

            // Assert
            await action.Should().ThrowAsync<ReservationException>();
        }

        [Test]
        public async Task Handle_WhenTicketIsNotExpired_ShouldThrowReservationException()
        {
            //Arrange
            var command = GetCreateReservationCommand();
            var showTimeEntity = new ShowtimeEntity();
            var tickets = new List<TicketEntity>()
        {
            new()
            {
                Paid = false,
                CreatedTime = DateTime.Now.AddMinutes(-1),
                TicketSeats = new List<TicketSeatsEntity>()
            }
        };

            mockShowtimeRepository.Setup(i => i.GetAllAsync(It.IsAny<Expression<Func<ShowtimeEntity, bool>>>(),
                                                            It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new[] { showTimeEntity });

            mockTicketsRepository.Setup(i => i.GetTicketsBySeats(It.IsAny<int>(),
                                                                 It.IsAny<List<SeatRequestModel>>(),
                                                                 It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => tickets);

            // Action
            Func<Task> action = async () => await sut.Handle(command, default);

            // Assert
            await action.Should().ThrowAsync<ReservationException>();
        }

        [Test]
        public async Task Handle_WhenTicketIsNotExpiredAndNotPaid_ShouldThrowReservationException()
        {
            //Arrange
            var command = GetCreateReservationCommand();
            var showTimeEntity = new ShowtimeEntity();
            var tickets = new List<TicketEntity>()
        {
            new()
            {
                Paid = false,
                CreatedTime = DateTime.Now.AddMinutes(-1),
                TicketSeats = new List<TicketSeatsEntity>()
            }
        };

            mockShowtimeRepository.Setup(i => i.GetAllAsync(It.IsAny<Expression<Func<ShowtimeEntity, bool>>>(),
                                                            It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new[] { showTimeEntity });

            mockTicketsRepository.Setup(i => i.GetTicketsBySeats(It.IsAny<int>(),
                                                                 It.IsAny<List<SeatRequestModel>>(),
                                                                 It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => tickets);

            // Action
            Func<Task> action = async () => await sut.Handle(command, default);

            // Assert
            await action.Should().ThrowAsync<ReservationException>();
        }


        [Test]
        public async Task Handle_WhenTicketEntityIsNull_ShouldThrowNotCreatedException()
        {
            //Arrange
            var command = GetCreateReservationCommand();
            var showTimeEntity = new ShowtimeEntity();
            var tickets = new List<TicketEntity>()
        {
            new()
            {
                Paid = false,
                CreatedTime = DateTime.Now.AddMinutes(-20),
                TicketSeats = new List<TicketSeatsEntity>()
            }
        };

            mockShowtimeRepository.Setup(i => i.GetAllAsync(It.IsAny<Expression<Func<ShowtimeEntity, bool>>>(),
                                                            It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(() => new[] { showTimeEntity });

            mockTicketsRepository.Setup(i => i.GetTicketsBySeats(It.IsAny<int>(),
                                                                 It.IsAny<List<SeatRequestModel>>(),
                                                                 It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(() => tickets);

            mockTicketsRepository.Setup(i => i.CreateAsync(It.IsAny<ShowtimeEntity>(),
                                                           It.IsAny<IEnumerable<SeatEntity>>(),
                                                           It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(() => null);

            // Action
            Func<Task> action = async () => await sut.Handle(command, default);

            // Assert
            await action.Should().ThrowAsync<NotCreatedException>();
        }

        [Test]
        public async Task Handle_WhenTicketEntityIdEmpty_ShouldThrowNotCreatedException()
        {
            //Arrange
            var command = GetCreateReservationCommand();
            var showTimeEntity = new ShowtimeEntity();
            var ticketEntity = new TicketEntity() { Id = Guid.Empty };
            var tickets = new List<TicketEntity>()
        {
            new()
            {
                Paid = false,
                CreatedTime = DateTime.Now.AddMinutes(-20),
                TicketSeats = new List<TicketSeatsEntity>()
            }
        };

            mockShowtimeRepository.Setup(i => i.GetAllAsync(It.IsAny<Expression<Func<ShowtimeEntity, bool>>>(),
                                                            It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(() => new[] { showTimeEntity });

            mockTicketsRepository.Setup(i => i.GetTicketsBySeats(It.IsAny<int>(),
                                                                 It.IsAny<List<SeatRequestModel>>(),
                                                                 It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(() => tickets);

            mockTicketsRepository.Setup(i => i.CreateAsync(It.IsAny<ShowtimeEntity>(),
                                                           It.IsAny<IEnumerable<SeatEntity>>(),
                                                           It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(() => ticketEntity);

            // Action
            Func<Task> action = () => sut.Handle(command, default);

            // Assert
            await action.Should().ThrowAsync<NotCreatedException>();
        }


        [Test]
        public async Task Handle_WhenEverythingIsValid_ShouldPass()
        {
            //Arrange
            var command = GetCreateReservationCommand();
            var showTimeEntity = new ShowtimeEntity();
            var ticketEntity = new TicketEntity() { Id = Guid.NewGuid() };
            var expectedViewModel = new TicketViewModel() { Id = Guid.NewGuid(), NumberOfSeats = 3 };
            var tickets = new List<TicketEntity>()
        {
            new(){ Paid = false, CreatedTime = DateTime.Now.AddMinutes(-20) }
        };

            mockShowtimeRepository.Setup(i => i.GetAllAsync(It.IsAny<Expression<Func<ShowtimeEntity, bool>>>(),
                                                            It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(() => new[] { showTimeEntity });

            mockTicketsRepository.Setup(i => i.GetTicketsBySeats(It.IsAny<int>(),
                                                                 It.IsAny<List<SeatRequestModel>>(),
                                                                 It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(() => tickets);

            mockTicketsRepository.Setup(i => i.CreateAsync(It.IsAny<ShowtimeEntity>(),
                                                           It.IsAny<IEnumerable<SeatEntity>>(),
                                                           It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(() => ticketEntity);

            mockMapper.Setup(i => i.Map<TicketViewModel>(It.IsAny<TicketEntity>()))
                      .Returns(expectedViewModel);

            // Action
            var response = await sut.Handle(command, default);

            // Assert
            response.Should().BeEquivalentTo(expectedViewModel);

        }

        private static CreateReservationCommand GetCreateReservationCommand(bool Contigious = true)
        {
            return new CreateReservationCommand
            {
                Seats = new List<SeatRequestModel>()
            {
                new(){ Row = 1, SeatNumber = 1 },
                new(){ Row = 1, SeatNumber = (short)(Contigious ? 2 : 3) }
            }
            };
        }
    }
}