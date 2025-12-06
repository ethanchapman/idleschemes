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

            var ticketClasses = await ExecuteAsyncAction<EventController, PublicTicketCollection>(c => c.GetAvailableTickets(ev.Id));
            ticketClasses.Classes.Count().ShouldBe(1);
            ticketClasses.Classes.First().Class.Name.ShouldBe("General");
            ticketClasses.Classes.First().All.ShouldBe(10);
            ticketClasses.Classes.First().Confirmed.ShouldBe(0);
            ticketClasses.Classes.First().Pending.ShouldBe(0);
            ticketClasses.Classes.First().Available.Count.ShouldBe(10);

            ev = evs.First(e => e.Name == "B&N Event 2");

            ticketClasses = await ExecuteAsyncAction<EventController, PublicTicketCollection>(c => c.GetAvailableTickets(ev.Id));
            ticketClasses.Classes.Count().ShouldBe(2);
            ticketClasses.Classes.First().Class.Name.ShouldBe("Class 1");
            ticketClasses.Classes.First().All.ShouldBe(5);
            ticketClasses.Classes.First().Confirmed.ShouldBe(0);
            ticketClasses.Classes.First().Pending.ShouldBe(0);
            ticketClasses.Classes.First().Available.Count.ShouldBe(5);
            ticketClasses.Classes.Last().Class.Name.ShouldBe("Class 2");
            ticketClasses.Classes.Last().All.ShouldBe(5);
            ticketClasses.Classes.Last().Confirmed.ShouldBe(0);
            ticketClasses.Classes.Last().Pending.ShouldBe(0);
            ticketClasses.Classes.Last().Available.Count.ShouldBe(5);
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

            var ticketClasses = await ExecuteAsyncAction<EventController, PublicTicketCollection>(c => c.GetAvailableTickets(ev.Id));
            ticketClasses.Classes.Count().ShouldBe(1);
            ticketClasses.Classes.First().Class.Name.ShouldBe("General");
            ticketClasses.Classes.First().All.ShouldBe(10);
            ticketClasses.Classes.First().Confirmed.ShouldBe(0);
            ticketClasses.Classes.First().Pending.ShouldBe(1);
            ticketClasses.Classes.First().Available.Count.ShouldBe(9);

            var confirmation = await ExecuteAsyncAction<RegistrationController, ActionResult>(c => c.ConfirmTickets(new ConfirmRegistrationModel {
                RegistrationId = registration.Id,
                RegistrationSecret = registration.Secret
            }), "ethan@idleschemes.com");

            ticketClasses = await ExecuteAsyncAction<EventController, PublicTicketCollection>(c => c.GetAvailableTickets(ev.Id));
            ticketClasses.Classes.Count().ShouldBe(1);
            ticketClasses.Classes.First().Class.Name.ShouldBe("General");
            ticketClasses.Classes.First().All.ShouldBe(10);
            ticketClasses.Classes.First().Confirmed.ShouldBe(1);
            ticketClasses.Classes.First().Pending.ShouldBe(0);
            ticketClasses.Classes.First().Available.Count.ShouldBe(9);

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

            var ticketClasses = await ExecuteAsyncAction<EventController, PublicTicketCollection>(c => c.GetAvailableTickets(ev.Id));
            ticketClasses.Classes.Count().ShouldBe(1);
            ticketClasses.Classes.First().Class.Name.ShouldBe("General");
            ticketClasses.Classes.First().All.ShouldBe(10);
            ticketClasses.Classes.First().Confirmed.ShouldBe(0);
            ticketClasses.Classes.First().Pending.ShouldBe(4);
            ticketClasses.Classes.First().Available.Count.ShouldBe(6);

            var confirmation = await ExecuteAsyncAction<RegistrationController, ActionResult>(c => c.ConfirmTickets(new ConfirmRegistrationModel {
                RegistrationId = registration.Id,
                RegistrationSecret = registration.Secret
            }), "ethan@idleschemes.com");

            ticketClasses = await ExecuteAsyncAction<EventController, PublicTicketCollection>(c => c.GetAvailableTickets(ev.Id));
            ticketClasses.Classes.Count().ShouldBe(1);
            ticketClasses.Classes.First().Class.Name.ShouldBe("General");
            ticketClasses.Classes.First().All.ShouldBe(10);
            ticketClasses.Classes.First().Confirmed.ShouldBe(4);
            ticketClasses.Classes.First().Pending.ShouldBe(0);
            ticketClasses.Classes.First().Available.Count.ShouldBe(6);

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

            var ticketClasses = await ExecuteAsyncAction<EventController, PublicTicketCollection>(c => c.GetAvailableTickets(ev.Id));

            ticketClasses.Classes.First().All.ShouldBe(5);
            ticketClasses.Classes.First().Pending.ShouldBe(1);
            ticketClasses.Classes.First().Available.Count.ShouldBe(4);
            ticketClasses.Classes.Last().All.ShouldBe(5);
            ticketClasses.Classes.Last().Pending.ShouldBe(2);
            ticketClasses.Classes.Last().Available.Count.ShouldBe(3);

            var confirmation = await ExecuteAsyncAction<RegistrationController, ActionResult>(c => c.ConfirmTickets(new ConfirmRegistrationModel {
                RegistrationId = registration.Id,
                RegistrationSecret = registration.Secret
            }), "ethan@idleschemes.com");

            ticketClasses = await ExecuteAsyncAction<EventController, PublicTicketCollection>(c => c.GetAvailableTickets(ev.Id));
            ticketClasses.Classes.First().All.ShouldBe(5);
            ticketClasses.Classes.First().Confirmed.ShouldBe(1);
            ticketClasses.Classes.First().Available.Count.ShouldBe(4);
            ticketClasses.Classes.Last().All.ShouldBe(5);
            ticketClasses.Classes.Last().Confirmed.ShouldBe(2);
            ticketClasses.Classes.Last().Available.Count.ShouldBe(3);

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

            var ticketClasses = await ExecuteAsyncAction<EventController, PublicTicketCollection>(c => c.GetAvailableTickets(ev.Id));
            ticketClasses.Classes.Last().All.ShouldBe(10);
            ticketClasses.Classes.Last().Pending.ShouldBe(0);
            ticketClasses.Classes.Last().Confirmed.ShouldBe(0);
            ticketClasses.Classes.Last().Available.Count.ShouldBe(10);

        }

    }
}
