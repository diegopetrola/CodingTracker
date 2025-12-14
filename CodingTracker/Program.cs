using CodingTracker;
using CodingTracker.Controller;
using CodingTracker.Data;
using CodingTracker.Services;

// Poor man's dependency injection. It's ugly and inelegant but it's my first time using DI by myself
// and, for the purposes of this project, I hope it's good enough. I am proud of my ugly, clunky child :)
// I will try a more robust version of DI next project.
var data = new CodingSessionData();
await data.CreateDatabase();
var service = new CodingSessionService(data);
var controller = new CodingSessionController(service);

await UserInterface.MainMenu(controller);
