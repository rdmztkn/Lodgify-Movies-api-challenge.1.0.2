using ApiApplication.Core.Application.Features.Commands.BuySeats;
using ApiApplication.Core.Application.Features.Commands.ConfirmReservation;
using ApiApplication.Core.Application.Repositories;
using ApiApplication.Core.Domain.Entities;
using ApiApplication.Exceptions;
using ApiApplication.Infrastructure.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiApplication.Tests.HandlerTests
{
    internal class ConfirmReservationCommandHandlerTests
    {
        private ConfirmReservationCommandHandler sut;
        private Mock<ITicketsRepository> mockTicketRepository;

        [SetUp]
        public void Setup()
        {
            mockTicketRepository = new();
            sut = new ConfirmReservationCommandHandler(mockTicketRepository.Object);
        }

        [Test]
        public async Task Handle_WhenRequestNull_ShouldThrowReservationException()
        {
            // Arrange

            // Action
            Func<Task> action = () => sut.Handle(null, default);

            // Assert
            await action.Should().ThrowAsync<ReservationException>();
        }

        [Test]
        public async Task Handle_WhenRequestEmptyGuid_ShouldThrowReservationException()
        {
            // Arrange
            var command = new ConfirmReservationCommand { ReservationId = Guid.Empty };

            // Action
            Func<Task> action = async () => await sut.Handle(command, default);

            // Assert
            await action.Should().ThrowAsync<ReservationException>();
        }

        [Test]
        public async Task Handle_WhenTicketEntityIsNull_ShouldThrowReservationException()
        {
            // Arrange
            var command = new ConfirmReservationCommand { ReservationId = Guid.NewGuid() };
            mockTicketRepository
                        .Setup(i => i.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(() => null);

            // Action 
            Func<Task> action = async () => await sut.Handle(command, default);

            // Assert
            await action.Should().ThrowAsync<ReservationException>().WithMessage("Reservation not found");
        }

        [Test]
        public async Task Handle_WhenTicketIsPaid_ShouldThrowReservationException()
        {
            // Arrange
            var ticketEntity = new TicketEntity
            {
                CreatedTime = DateTime.Now.AddMinutes(-2),
                Paid = true
            };

            var command = new ConfirmReservationCommand { ReservationId = Guid.NewGuid() };
            mockTicketRepository
                        .Setup(i => i.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(() => ticketEntity);

            // Action 
            Func<Task> action = async () => await sut.Handle(command, default);

            // Assert
            await action.Should().ThrowAsync<ReservationException>().WithMessage("Reservation already paid");
        }

        [Test]
        public async Task Handle_WhenTicketExpired_ShouldThrowReservationException()
        {
            // Arrange
            var ticketEntity = new TicketEntity
            {
                CreatedTime = DateTime.Now.AddHours(-2)
            };

            var command = new ConfirmReservationCommand { ReservationId = Guid.NewGuid() };
            mockTicketRepository
                        .Setup(i => i.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(() => ticketEntity);

            // Action 
            Func<Task> action = async () => await sut.Handle(command, default);

            // Assert
            await action.Should().ThrowAsync<ReservationException>().WithMessage("Reservation expired");
        }


        [Test]
        public async Task Handle_WhenConfirmReturnNull_ShouldThrowReservationException()
        {
            // Arrange
            var ticketEntity = new TicketEntity
            {
                CreatedTime = DateTime.Now.AddMinutes(-2),
                Paid = false
            };

            var command = new ConfirmReservationCommand { ReservationId = Guid.NewGuid() };

            mockTicketRepository
                        .Setup(i => i.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(() => ticketEntity);

            mockTicketRepository.Setup(i => i.ConfirmPaymentAsync(ticketEntity, It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => null);

            // Action 
            Func<Task> action = async () => await sut.Handle(command, default);

            // Assert
            await action.Should().ThrowAsync<NotCreatedException>().WithMessage("Reservation couldn't confirmed");
        }

        [Test]
        public async Task Handle_WhenConfirmReturnEmptyId_ShouldThrowReservationException()
        {
            // Arrange
            var ticketEntity = new TicketEntity
            {
                CreatedTime = DateTime.Now.AddMinutes(-2),
                Paid = false
            };

            var ticketEntityReturned = new TicketEntity { Id = Guid.Empty };

            var command = new ConfirmReservationCommand { ReservationId = Guid.NewGuid() };

            mockTicketRepository
                        .Setup(i => i.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(() => ticketEntity);

            mockTicketRepository.Setup(i => i.ConfirmPaymentAsync(ticketEntity, It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => ticketEntityReturned);

            // Action 
            Func<Task> action = async () => await sut.Handle(command, default);

            // Assert
            await action.Should().ThrowAsync<NotCreatedException>().WithMessage("Reservation couldn't confirmed");
        }

        [Test]
        public async Task Handle_WhenEverythingIsValid_ShouldPass()
        {
            // Arrange
            var ticketEntity = new TicketEntity
            {
                CreatedTime = DateTime.Now.AddMinutes(-2),
                Paid = false
            };

            var ticketEntityReturned = new TicketEntity { Id = Guid.NewGuid() };

            var command = new ConfirmReservationCommand { ReservationId = Guid.NewGuid() };

            mockTicketRepository
                        .Setup(i => i.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(() => ticketEntity);

            mockTicketRepository.Setup(i => i.ConfirmPaymentAsync(ticketEntity, It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => ticketEntityReturned);

            // Action 
            var result = await sut.Handle(command, default);

            // Assert
            result.Should().BeTrue();
        }

    }
}