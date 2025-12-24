# E-commerce Web API

A full-stack e-commerce backend built with ASP.NET Core, featuring layered architecture and event-driven processing.

Frontend GitHub Repo: https://github.com/Samiul2651/EcommerceF

## Architecture

| Layer | Description |
|-------|-------------|
| **EcommerceWebApi** | REST API endpoints |
| **Business** | Business logic and services |
| **Contracts** | DTOs and interfaces |
| **Receive** | Message queue consumer for async processing |

## Tech Stack

- ASP.NET Core Web API
- RabbitMQ
- Liquid (email templating)
- Docker & Docker Compose

## Features

- RESTful API endpoints
- Layered architecture with separation of concerns
- Asynchronous message processing with RabbitMQ
- Email notifications using Liquid templates
- Multi-container Docker deployment
