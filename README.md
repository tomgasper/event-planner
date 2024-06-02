# EventPlanner

EventPlanner is a web application built with ASP.NET MVC, Entity Framework, and .NET Identity. It allows users to create, manage, and view events. The application also supports user authentication and authorization.

## Features

- [x] User registration and authentication using .NET Identity
- [x] Role-based access control
- [ ] Manage event locations with relational data (country, city, street, location)
- [ ] Create, read, update, and delete events
- [ ] List of categories with dropdown selection
- [ ] Notifications for existing events with the same details
- [ ] Asynchronous data fetching and processing

## Technologies Used

- ASP.NET MVC
- Entity Framework Core
- .NET Identity
- Bootstrap (for UI styling)
- SQL Server (default database)

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