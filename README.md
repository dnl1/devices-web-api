# Devices API

A simple RESTful API for managing devices, built with **.NET 9**, **PostgreSQL**, and **Docker**.

This project is intended for **local development and testing purposes**.

---

## Requirements

Make sure you have the following installed:

- **.NET 9 SDK**
- **Docker & Docker Compose**
- **PostgreSQL**

---

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/dnl1/devices-web-api
cd devices-web-api
```

### 2. Create the .env file

This project uses environment variables for local development.

Create a .env file in the root of the project with the following structure:

```bash
POSTGRES_DB=DeviceDB
POSTGRES_USER=postgres
POSTGRES_PASSWORD=your_db_password
```

### 3. Start the database using Docker Compose

From the project root, run:

```bash
docker-compose up -d
```

This will:

- Start a PostgreSQL container
- Create the database defined in .env
- Expose PostgreSQL on port 5433 (I did that cause I have a local Postgres running on default port 5432)
- Expose the Web API over https://localhost:8080

You can access the Swagger page by
https://localhost:8080/swagger