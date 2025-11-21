using IdleSchemes.Api.Controllers;
using IdleSchemes.Api.Models;
using IdleSchemes.Api.Models.Input;
using IdleSchemes.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace IdleSchemes.Tests.Api {
    public class RegistrationTests : TestBase {

        [Test]
        public async Task TestInitialAvailability() {
            var evs = await ExecuteAsyncAction<EventController, IEnumerable<EventInstanceModel>>(c => c.GetEventsInRegion("den"));
            var ev = evs.First(e => e.Name == "B&N Event 1");

            var ticketClasses = await ExecuteAsyncAction<EventController, IEnumerable<TicketClassModel>>(c => c.GetAvailableTickets(ev.Id));
            ticketClasses.Count().ShouldBe(1);
            ticketClasses.First().Name.ShouldBe("General");
            ticketClasses.First().Availability!.TotalCount.ShouldBe(10);
            ticketClasses.First().Availability!.ConfirmedCount.ShouldBe(0);
            ticketClasses.First().Availability!.PendingCount.ShouldBe(0);
            ticketClasses.First().Availability!.RemainingCount.ShouldBe(10);

            ev = evs.First(e => e.Name == "B&N Event 2");

            ticketClasses = await ExecuteAsyncAction<EventController, IEnumerable<TicketClassModel>>(c => c.GetAvailableTickets(ev.Id));
            ticketClasses.Count().ShouldBe(2);
            ticketClasses.First().Name.ShouldBe("Class 1");
            ticketClasses.First().Availability!.TotalCount.ShouldBe(5);
            ticketClasses.First().Availability!.ConfirmedCount.ShouldBe(0);
            ticketClasses.First().Availability!.PendingCount.ShouldBe(0);
            ticketClasses.First().Availability!.RemainingCount.ShouldBe(5);
            ticketClasses.Last().Name.ShouldBe("Class 2");
            ticketClasses.Last().Availability!.TotalCount.ShouldBe(5);
            ticketClasses.Last().Availability!.ConfirmedCount.ShouldBe(0);
            ticketClasses.Last().Availability!.PendingCount.ShouldBe(0);
            ticketClasses.Last().Availability!.RemainingCount.ShouldBe(5);
        }

        [Test]
        public async Task TestRegisterSingleTicket() {
            var evs = await ExecuteAsyncAction<EventController, IEnumerable<EventInstanceModel>>(c => c.GetEventsInRegion("den"));
            var ev = evs.First(e => e.Name == "B&N Event 1");

            var registration = await ExecuteAsyncAction<RegistrationController, RegistrationInfoModel>(c => c.StartRegistration(new StartRegistrationModel { 
                InstanceId = ev.Id,
            }), "ethan@idleschemes.com");

            var reservation = await ExecuteAsyncAction<RegistrationController, ReservationInfoModel>(c => c.ReserveTickets(new ReserveTicketsModel {
                RegistrationId = registration.Id,
                RegistrationSecret = registration.Secret,
                TicketReservations = [new ReserveTicketsModel.TicketReservation { Class = "General", Count = 1 }]
            }), "ethan@idleschemes.com");

            var ticketClasses = await ExecuteAsyncAction<EventController, IEnumerable<TicketClassModel>>(c => c.GetAvailableTickets(ev.Id));
            ticketClasses.Count().ShouldBe(1);
            ticketClasses.First().Name.ShouldBe("General");
            ticketClasses.First().Availability!.TotalCount.ShouldBe(10);
            ticketClasses.First().Availability!.ConfirmedCount.ShouldBe(0);
            ticketClasses.First().Availability!.PendingCount.ShouldBe(1);
            ticketClasses.First().Availability!.ConfirmedAndPendingCount.ShouldBe(1);
            ticketClasses.First().Availability!.RemainingCount.ShouldBe(9);

            var confirmation = await ExecuteAsyncAction<RegistrationController, ActionResult>(c => c.ConfirmTickets(new ConfirmRegistrationModel {
                RegistrationId = registration.Id,
                RegistrationSecret = registration.Secret
            }), "ethan@idleschemes.com");

            ticketClasses = await ExecuteAsyncAction<EventController, IEnumerable<TicketClassModel>>(c => c.GetAvailableTickets(ev.Id));
            ticketClasses.Count().ShouldBe(1);
            ticketClasses.First().Name.ShouldBe("General");
            ticketClasses.First().Availability!.TotalCount.ShouldBe(10);
            ticketClasses.First().Availability!.ConfirmedCount.ShouldBe(1);
            ticketClasses.First().Availability!.PendingCount.ShouldBe(0);
            ticketClasses.First().Availability!.ConfirmedAndPendingCount.ShouldBe(1);
            ticketClasses.First().Availability!.RemainingCount.ShouldBe(9);

        }

        [Test]
        public async Task TestRegisterMultipleTicketsOfSameClass() {
            var evs = await ExecuteAsyncAction<EventController, IEnumerable<EventInstanceModel>>(c => c.GetEventsInRegion("den"));
            var ev = evs.First(e => e.Name == "B&N Event 1");

            var registration = await ExecuteAsyncAction<RegistrationController, RegistrationInfoModel>(c => c.StartRegistration(new StartRegistrationModel {
                InstanceId = ev.Id,
            }), "ethan@idleschemes.com");

            var reservation = await ExecuteAsyncAction<RegistrationController, ReservationInfoModel>(c => c.ReserveTickets(new ReserveTicketsModel {
                RegistrationId = registration.Id,
                RegistrationSecret = registration.Secret,
                TicketReservations = [new ReserveTicketsModel.TicketReservation { Class = "General", Count = 4 }]
            }), "ethan@idleschemes.com");

            var ticketClasses = await ExecuteAsyncAction<EventController, IEnumerable<TicketClassModel>>(c => c.GetAvailableTickets(ev.Id));
            ticketClasses.Count().ShouldBe(1);
            ticketClasses.First().Name.ShouldBe("General");
            ticketClasses.First().Availability!.TotalCount.ShouldBe(10);
            ticketClasses.First().Availability!.ConfirmedCount.ShouldBe(0);
            ticketClasses.First().Availability!.PendingCount.ShouldBe(4);
            ticketClasses.First().Availability!.ConfirmedAndPendingCount.ShouldBe(4);
            ticketClasses.First().Availability!.RemainingCount.ShouldBe(6);

            var confirmation = await ExecuteAsyncAction<RegistrationController, ActionResult>(c => c.ConfirmTickets(new ConfirmRegistrationModel {
                RegistrationId = registration.Id,
                RegistrationSecret = registration.Secret
            }), "ethan@idleschemes.com");

            ticketClasses = await ExecuteAsyncAction<EventController, IEnumerable<TicketClassModel>>(c => c.GetAvailableTickets(ev.Id));
            ticketClasses.Count().ShouldBe(1);
            ticketClasses.First().Name.ShouldBe("General");
            ticketClasses.First().Availability!.TotalCount.ShouldBe(10);
            ticketClasses.First().Availability!.ConfirmedCount.ShouldBe(4);
            ticketClasses.First().Availability!.PendingCount.ShouldBe(0);
            ticketClasses.First().Availability!.ConfirmedAndPendingCount.ShouldBe(4);
            ticketClasses.First().Availability!.RemainingCount.ShouldBe(6);

        }

        [Test]
        public async Task TestRegisterMultipleTicketsOfDifferentClass() {
            var evs = await ExecuteAsyncAction<EventController, IEnumerable<EventInstanceModel>>(c => c.GetEventsInRegion("den"));
            var ev = evs.First(e => e.Name == "B&N Event 2");

            var registration = await ExecuteAsyncAction<RegistrationController, RegistrationInfoModel>(c => c.StartRegistration(new StartRegistrationModel {
                InstanceId = ev.Id,
            }), "ethan@idleschemes.com");

            var reservation = await ExecuteAsyncAction<RegistrationController, ReservationInfoModel>(c => c.ReserveTickets(new ReserveTicketsModel {
                RegistrationId = registration.Id,
                RegistrationSecret = registration.Secret,
                TicketReservations = [new ReserveTicketsModel.TicketReservation { Class = "Class 1", Count = 1 }, new ReserveTicketsModel.TicketReservation { Class = "Class 2", Count = 2 }]
            }), "ethan@idleschemes.com");

            var ticketClasses = await ExecuteAsyncAction<EventController, IEnumerable<TicketClassModel>>(c => c.GetAvailableTickets(ev.Id));
            ticketClasses.First().Availability!.TotalCount.ShouldBe(5);
            ticketClasses.First().Availability!.PendingCount.ShouldBe(1);
            ticketClasses.First().Availability!.RemainingCount.ShouldBe(4);
            ticketClasses.Last().Availability!.TotalCount.ShouldBe(5);
            ticketClasses.Last().Availability!.PendingCount.ShouldBe(2);
            ticketClasses.Last().Availability!.RemainingCount.ShouldBe(3);

            var confirmation = await ExecuteAsyncAction<RegistrationController, ActionResult>(c => c.ConfirmTickets(new ConfirmRegistrationModel {
                RegistrationId = registration.Id,
                RegistrationSecret = registration.Secret
            }), "ethan@idleschemes.com");

            ticketClasses = await ExecuteAsyncAction<EventController, IEnumerable<TicketClassModel>>(c => c.GetAvailableTickets(ev.Id));
            ticketClasses.First().Availability!.TotalCount.ShouldBe(5);
            ticketClasses.First().Availability!.ConfirmedCount.ShouldBe(1);
            ticketClasses.First().Availability!.RemainingCount.ShouldBe(4);
            ticketClasses.Last().Availability!.TotalCount.ShouldBe(5);
            ticketClasses.Last().Availability!.ConfirmedCount.ShouldBe(2);
            ticketClasses.Last().Availability!.RemainingCount.ShouldBe(3);

        }

        [Test]
        public async Task TestUserRegistrations() {
            var evs = await ExecuteAsyncAction<EventController, IEnumerable<EventInstanceModel>>(c => c.GetEventsInRegion("den"));
            var ev = evs.First(e => e.Name == "B&N Event 1");

            var registrations = await ExecuteAsyncAction<UserEventsController, IEnumerable<UserRegistrationModel>>(c => c.GetMyEventRegistrations(),
                "ethan@idleschemes.com");
            registrations.Count().ShouldBe(0);

            var registration = await ExecuteAsyncAction<RegistrationController, RegistrationInfoModel>(c => c.StartRegistration(new StartRegistrationModel {
                InstanceId = ev.Id,
            }), "ethan@idleschemes.com");

            registrations = await ExecuteAsyncAction<UserEventsController, IEnumerable<UserRegistrationModel>>(c => c.GetMyEventRegistrations(),
                "ethan@idleschemes.com");
            registrations.Count().ShouldBe(0);

            var reservation = await ExecuteAsyncAction<RegistrationController, ReservationInfoModel>(c => c.ReserveTickets(new ReserveTicketsModel {
                RegistrationId = registration.Id,
                RegistrationSecret = registration.Secret,
                TicketReservations = [new ReserveTicketsModel.TicketReservation { Class = "General", Count = 4 }]
            }), "ethan@idleschemes.com");

            registrations = await ExecuteAsyncAction<UserEventsController, IEnumerable<UserRegistrationModel>>(c => c.GetMyEventRegistrations(),
                "ethan@idleschemes.com");
            registrations.Count().ShouldBe(0);

            var confirmation = await ExecuteAsyncAction<RegistrationController, ActionResult>(c => c.ConfirmTickets(new ConfirmRegistrationModel {
                RegistrationId = registration.Id,
                RegistrationSecret = registration.Secret
            }), "ethan@idleschemes.com");

            registrations = await ExecuteAsyncAction<UserEventsController, IEnumerable<UserRegistrationModel>>(c => c.GetMyEventRegistrations(),
                "ethan@idleschemes.com");
            registrations.Count().ShouldBe(1);
            registrations.First().Tickets.Count.ShouldBe(4);

        }

        [Test]
        public async Task TestCancellingRegistration() {
            var evs = await ExecuteAsyncAction<EventController, IEnumerable<EventInstanceModel>>(c => c.GetEventsInRegion("den"));
            var ev = evs.First(e => e.Name == "B&N Event 1");

            var registration = await ExecuteAsyncAction<RegistrationController, RegistrationInfoModel>(c => c.StartRegistration(new StartRegistrationModel {
                InstanceId = ev.Id,
            }), "ethan@idleschemes.com");

            var reservation = await ExecuteAsyncAction<RegistrationController, ReservationInfoModel>(c => c.ReserveTickets(new ReserveTicketsModel {
                RegistrationId = registration.Id,
                RegistrationSecret = registration.Secret,
                TicketReservations = [new ReserveTicketsModel.TicketReservation { Class = "General", Count = 4 }]
            }), "ethan@idleschemes.com");

            var confirmation = await ExecuteAsyncAction<RegistrationController, ActionResult>(c => c.ConfirmTickets(new ConfirmRegistrationModel {
                RegistrationId = registration.Id,
                RegistrationSecret = registration.Secret
            }), "ethan@idleschemes.com");

            var registrations = await ExecuteAsyncAction<UserEventsController, IEnumerable<UserRegistrationModel>>(c => c.GetMyEventRegistrations(),
                "ethan@idleschemes.com");

            await ExecuteAsyncAction<UserEventsController, ActionResult>(c => c.CancelEventRegistration(registrations.First().Id),
                "ethan@idleschemes.com");

            var ticketClasses = await ExecuteAsyncAction<EventController, IEnumerable<TicketClassModel>>(c => c.GetAvailableTickets(ev.Id));
            ticketClasses.First().Availability!.TotalCount.ShouldBe(10);
            ticketClasses.First().Availability!.PendingCount.ShouldBe(0);
            ticketClasses.First().Availability!.ConfirmedCount.ShouldBe(0);
            ticketClasses.First().Availability!.RemainingCount.ShouldBe(10);

        }

    }
}
