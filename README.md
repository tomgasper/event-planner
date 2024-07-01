# EventPlanner

EventPlanner is a web application built with ASP.NET MVC, Entity Framework, and .NET Identity. The app provides an interface for users to create, manage, and discover various types of events.

It includes search functionality that allows users to filter events based on specific criteria.

The application also features a user profile page where users can manage their own events and adjust settings related to their account.

![Events page](https://github.com/tomgasper/event-planner/blob/master/Screenshots/page_1.jpg?raw=true)

![Event details page](https://github.com/tomgasper/event-planner/blob/master/Screenshots/page_2.jpg?raw=true)

![Events list](https://github.com/tomgasper/event-planner/blob/master/Screenshots/page_4.jpg?raw=true)

## Features

- [x] User registration and authentication using .NET Identity
- [x] Register, login, and manage user profiles.
- [x] Upload user profile
- [x] Create, read, update, and delete events
- [x] Event Managment page
- [x] List of categories with dropdown selection
- [x] Profile Settings (Account Delection, Account Visibility and Login History)
- [ ] Responsive UI (to do)

## Technologies Used

- ASP.NET MVC
- Entity Framework Core
- .NET Identity
- SQL Server (default database)
- xUnit, FluentAssertions, NSubstitute

## Getting Started

Follow these steps to set up and run the project locally.

### Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download)
- [Visual Studio](https://visualstudio.microsoft.com/) (or any preferred IDE)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or SQL Server Express)

### Installation

1. **Clone the repository:**

    ```sh
    git clone https://github.com/yourusername/EventPlanner.git
    cd EventPlanner
    ```

2. **Set up the database:**

    Update the connection string in `appsettings.json` to point to your SQL Server instance.

    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=your_server;Database=EventPlannerDb;Trusted_Connection=True;MultipleActiveResultSets=true"
    }
    ```

3. **Apply migrations:**

    Open the terminal in the project directory and run the following commands to create the database and apply the migrations.

    ```sh
    dotnet ef migrations add InitialCreate
    dotnet ef database update
    ```

4. **Run the application:**

    ```sh
    dotnet run
    ```

    Or you can use Visual Studio to run the project by pressing `F5`.