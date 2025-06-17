# Movie Booking API

A .NET 8 Web API for a movie ticket booking system with features like user authentication, movie listings, theater management, seat selection, and payment processing using Stripe.

## Features

- User Authentication with JWT
- Movie and Theater Management
- Show Time Scheduling
- Seat Selection and Locking
- Booking Management
- Payment Integration with Stripe
- Swagger Documentation

## Prerequisites

- .NET 8.0 SDK
- SQLite
- Stripe Account (for payment processing)

## Getting Started

1. Clone the repository:
   ```bash
   git clone https://github.com/raghav2404/CursorAiMovieBookingApp.git
   cd CursorAiMovieBookingApp/MovieBooking.API
   ```

2. Install dependencies:
   ```bash
   dotnet restore
   ```

3. Update the connection string in `appsettings.json` if needed.

4. Apply database migrations:
   ```bash
   dotnet ef database update
   ```

5. Run the application:
   ```bash
   dotnet run
   ```

The API will be available at:
- HTTP: http://localhost:5001
- HTTPS: https://localhost:7096

## API Documentation

Access the Swagger UI documentation at the root URL (http://localhost:5001 or https://localhost:7096) when running in development mode.

### Main Endpoints

- **Auth**
  - POST /api/Auth/register - Register new user
  - POST /api/Auth/login - Login and get JWT token

- **Movies**
  - GET /api/Movies - List all movies
  - GET /api/Movies/{id} - Get movie details
  - POST /api/Movies - Add new movie (Admin)
  - PUT /api/Movies/{id} - Update movie (Admin)
  - DELETE /api/Movies/{id} - Delete movie (Admin)

- **Theaters**
  - GET /api/Theaters - List all theaters
  - GET /api/Theaters/{id} - Get theater details
  - POST /api/Theaters - Add new theater (Admin)
  - PUT /api/Theaters/{id} - Update theater (Admin)
  - DELETE /api/Theaters/{id} - Delete theater (Admin)

- **ShowTimes**
  - GET /api/ShowTimes - List all showtimes
  - GET /api/ShowTimes/{id} - Get showtime details
  - POST /api/ShowTimes - Add new showtime (Admin)

- **Bookings**
  - GET /api/Bookings - List user's bookings
  - POST /api/Bookings - Create new booking
  - GET /api/Bookings/{id} - Get booking details

## Authentication

The API uses JWT Bearer token authentication. To access protected endpoints:

1. Register a new user using `/api/Auth/register`
2. Login using `/api/Auth/login` to get the JWT token
3. Include the token in the Authorization header:
   ```
   Authorization: Bearer <your-token>
   ```

## Database

The application uses SQLite as the database. The connection string can be configured in `appsettings.json`.

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details. 