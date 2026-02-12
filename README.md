# Campus Insider

Campus Insider is designed to facilitate resource sharing and collaboration within a university campus.

The platform focuses on three main features:
- Equipment sharing between campus members
- Peer-to-peer equipment loans
- Carpool ride organization
- Messaging Platform
- Real time Notifications System

This project is currently under active development and is intended to demonstrate backend architecture and business-oriented design.

---

## Core Features

### Equipment Sharing
Users can share personal equipment with others on campus, manage availability, and update equipment details.

### Equipment Loans
Users can request to borrow shared equipment. Loans follow a lifecycle with clear states such as pending, approved, rejected, extended, and completed.

### Carpooling
Users can create carpool trips, join or leave rides, and manage seat availability.

---

## Technical Stack

- ASP.NET Core (Web API)
- Entity Framework Core
- PostgreSQL
- MVC architecture (Controllers, Services, DTOs, Models)

---

## Architecture Overview

- Controllers handle HTTP requests and responses
- Services contain business logic
- Models entities represent core concepts (User, Equipment, Loan, CarpoolTrip)
- Database schema designed with relational integrity

UML diagrams are used to model entities, relationships, and business operations.
---
<p align="center"> <img src="preview/highleveldiagram.png" width="45%" /><img src="preview/db_uml_campusinsider.png" width="45%" />  </p>

## Project Status

Ongoing development  
