# Task Gauge Project

Task Gauge is a Scrum poker application designed for use in package kickoff meetings. It was built to allow participants to make and join rooms in real time and provide predictions for opened tasks. The project used MVC architecture, SignalR for real-time communication and Entity Framework for data management.

Note: To optimize performance, the effort estimates provided for tasks opened during the meeting are temporarily stored in a static list. These predictions are added to the database only when the room is closed, minimizing frequent interactions with the database.

## Features

- Real-time communication with SignalR
- Room make and management
- User authentication and participation
- Real-time task estimations
- MVC (Model-View-Controller) design pattern
- Use of Entity Framework
- Role-Based Access Control and room participation

## How the Project Works

1. Open the `TaskGauge.sln` file in the project's root directory.
2. Compile the project using Visual Studio or your preferred IDE.
3. Run the `TaskGauge.Mvc` project to start the application.

## Usage

1. Log in or register.
2. Make a room or join an existing room when the application starts.
3. Participants in the room can provide their effort estimates for opened tasks.
4. Observe real-time estimations from other participants.
5. At the end of the meeting, generate a report regarding the estimations.

## Technologies Used

- ASP.NET Core MVC
- SignalR (For Real-Time Communication)
- Entity Framework (Code-First Approach)
- Cookie-based Authentication
- SweetAlert2
- Inversion Of Control
- Layered Architecture
- Html & Css & Bootstrap & JavaScript & jQuery

## Contribution

If you would like to contribute to this project, please submit a pull request. Your contributions are welcome!
