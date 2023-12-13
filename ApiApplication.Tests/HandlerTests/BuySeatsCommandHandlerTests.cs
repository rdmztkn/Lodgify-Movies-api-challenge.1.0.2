using ApiApplication.Core.Application.Features.Commands.BuySeats;
using ApiApplication.Core.Application.Models.RequestModels;
using ApiApplication.Core.Application.Repositories;
using ApiApplication.Core.Domain.Entities;
using ApiApplication.Exceptions;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiApplication.Tests.HandlerTests
{
    public class BuySeatsCommandHandlerTests
    {
        private BuySeatsCommandHandler sut; // System Under Test
        private Mock<ITicketsRepository> mockTicketRepository;

        [SetUp]
        public void Setup()
        {
            mockTicketRepository = new();

            sut = new BuySeatsCommandHandler(mockTicketRepository.Object);
        }

        [Test]
        public async Task Handle_WhenRequestNull_ShouldThrowReservationException()
        {
            // Arrange

            // Action
            Func<Task> action = async () => await sut.Handle(null, default);

            // Assert
            await action.Should().ThrowAsync<ReservationException>();
        }

        [Test]
        public async Task Handle_WhenRequestEmptyGuid_ShouldThrowReservationException()
        {
            // Arrange
            var command = new BuySeatsCommand { ReservationId = Guid.Empty };

            // Action
            Func<Task> action = async () => await sut.Handle(command, default);

            // Assert
            await action.Should().ThrowAsync<ReservationException>();
        }

        [Test]
        public async Task Handle_WhenTicketEntityIsNull_ShouldThrowReservationException()
        {
            // Arrange
            var command = new BuySeatsCommand { ReservationId = Guid.NewGuid() };
            mockTicketRepository
                        .Setup(i => i.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(() => null);

            // Action 
            Func<Task> action = async () => await sut.Handle(command, default);

            // Assert
            await action.Should().ThrowAsync<ReservationException>().WithMessage("Reservation not found");
        }

        [Test]
        public async Task Handle_WhenTicketExpired_ShouldThrowReservationException()
        {
            // Arrange
            var ticketEntity = new TicketEntity
            {
                CreatedTime = DateTime.Now.AddHours(-2)
            };

            var command = new BuySeatsCommand { ReservationId = Guid.NewGuid() };
            mockTicketRepository
                        .Setup(i => i.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(() => ticketEntity);

            // Action 
            Func<Task> action = async () => await sut.Handle(command, default);

            // Assert
            await action.Should().ThrowAsync<ReservationException>().WithMessage("Reservation expired");
        }

        [Test]
        public async Task Handle_WhenEverythingIsValid_ShouldPass()
        {
            // Arrange
            var ticketEntity = new TicketEntity
            {
                CreatedTime = DateTime.Now.AddMinutes(1)
            };

            var command = new BuySeatsCommand { ReservationId = Guid.NewGuid() };
            mockTicketRepository
                        .Setup(i => i.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(() => ticketEntity);

            // Action 
            var result = await sut.Handle(command, default);

            // Assert
            result.Should().BeTrue();
        }
    }
}